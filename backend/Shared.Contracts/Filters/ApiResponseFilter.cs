using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Contracts.Responses;

namespace Shared.Contracts.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // NoContentResult (204) / FileResult / RedirectResult cố tình không bọc:
            // 204 không được có body, các loại còn lại đã tự stream nội dung riêng.
            if (context.Result is not ObjectResult objectResult)
            {
                return;
            }

            // Đã bọc dạng ApiResponse sẵn (ví dụ từ controller hoặc
            // InvalidModelStateResponseFactory) -> không bọc lại, chỉ bổ sung traceId.
            //
            // Phải so khớp theo generic type definition chứ không dùng
            // `is ApiResponse<object>`: ApiResponse<ProfileDto> không kế thừa
            // ApiResponse<object> nên pattern matching sẽ fail và gây wrap hai lớp.
            if (objectResult.Value != null && IsApiResponse(objectResult.Value))
            {
                BackfillTraceId(objectResult.Value, context.HttpContext.TraceIdentifier);
                return;
            }

            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

            // Chỉ 2xx mới là success. Trước đây dùng "< 400" khiến 3xx bị gắn success: true.
            if (statusCode >= 200 && statusCode < 300)
            {
                var response = ApiResponse.SuccessResult(objectResult.Value);
                response.TraceId = context.HttpContext.TraceIdentifier;
                context.Result = new ObjectResult(response) { StatusCode = statusCode };
            }
            else
            {
                var (message, errors) = ExtractErrorDetails(objectResult.Value);
                var response = ApiResponse.FailResult(message, errors);
                response.TraceId = context.HttpContext.TraceIdentifier;
                context.Result = new ObjectResult(response) { StatusCode = statusCode };
            }
        }

        private static bool IsApiResponse(object obj)
        {
            var type = obj.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                return true;
            return type == typeof(ApiResponse);
        }

        private static void BackfillTraceId(object response, string traceId)
        {
            if (response is IApiResponse apiResponse && apiResponse.TraceId == null)
            {
                apiResponse.TraceId = traceId;
            }
        }

        private static (string Message, List<string> Errors) ExtractErrorDetails(object? value)
        {
            if (value == null) return ("Đã xảy ra lỗi.", new List<string>());

            if (value is string str)
            {
                // Không nhân bản message vào errors: message đã mô tả lỗi rồi,
                // để tránh client hiển thị trùng 2 lần.
                return (str, new List<string>());
            }

            if (value is ValidationProblemDetails validationDetails)
            {
                var errorsList = validationDetails.Errors
                    .SelectMany(e => e.Value.Select(msg => $"{e.Key}: {msg}"))
                    .ToList();
                return ("Dữ liệu đầu vào không hợp lệ.", errorsList);
            }

            if (value is SerializableError serializableError)
            {
                var errorsList = serializableError
                    .SelectMany(e => ((string[])e.Value).Select(msg => $"{e.Key}: {msg}"))
                    .ToList();
                return ("Dữ liệu đầu vào không hợp lệ.", errorsList);
            }

            var prop = value.GetType().GetProperty("message") ?? value.GetType().GetProperty("Message");
            if (prop != null)
            {
                var msgVal = prop.GetValue(value)?.ToString();
                if (!string.IsNullOrEmpty(msgVal))
                {
                    return (msgVal, new List<string>());
                }
            }

            // Không dùng value.ToString(): với anonymous object sẽ trả về tên type
            // kiểu "<>f__AnonymousType0`1" và làm message thành rác.
            return ("Đã xảy ra lỗi.", new List<string>());
        }
    }
}