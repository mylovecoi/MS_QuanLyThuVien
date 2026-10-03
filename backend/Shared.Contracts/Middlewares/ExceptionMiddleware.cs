using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Responses;

namespace Shared.Contracts.Middlewares
{
    public class ExceptionMiddleware
    {
        private const string GenericErrorMessage = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Phân biệt lỗi nghiệp vụ với lỗi hệ thống ngay tại đây.
                //
                // AppException là lỗi đã được dự kiến (sai dữ liệu, sai mật
                // khẩu, không tìm thấy...). Ghi nó ở mức Error kèm stack trace và
                // nhãn "Unhandled exception" là sai lệch: nó làm log trông như
                // app sập, dễ khiến người đọc tưởng có bug trong khi thực tế
                // frontend sắp nhận HTTP 400/401/404 đúng như thiết kế.
                //
                // Lỗi hệ thống thật sự (null reference, lỗi SQL,...) mới là
                // "unhandled" và mới cần stack trace để debug.
                if (ex is AppException appEx)
                {
                    _logger.LogWarning(
                        appEx,
                        "Lỗi nghiệp vụ {StatusCode}. TraceId: {TraceId}. Message: {Message}. Errors: {Errors}",
                        appEx.StatusCode,
                        context.TraceIdentifier,
                        appEx.Message,
                        appEx.Errors != null && appEx.Errors.Count > 0
                            ? string.Join(" | ", appEx.Errors)
                            : "(không có)");
                }
                else
                {
                    _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);
                }

                await HandleExceptionAsync(context, ex, _env);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment env)
        {
            // Response đã bắt đầu stream thì không ghi được header/status nữa.
            // Ghi thêm sẽ khiến chính middleware này fault với InvalidOperationException.
            if (context.Response.HasStarted)
            {
                return;
            }

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = GenericErrorMessage;
            var errors = new List<string>();

            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                if (appEx.Errors != null && appEx.Errors.Count > 0)
                {
                    errors.AddRange(appEx.Errors);
                }
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Không có quyền truy cập.";
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = "Không tìm thấy tài nguyên yêu cầu.";
            }

            // Không gán exception.Message cho lỗi 500: message của SqlException /
            // DbUpdateException có thể chứa tên server, tên database, thông tin kết nối.
            // Chi tiết đã được ghi log ở InvokeAsync, client chỉ cần traceId để tra log.

            // AppException là lỗi nghiệp vụ đã được chủ định: message của nó an toàn
            // để trả ra ngoài, không cần kèm stack trace kể cả ở Development.
            //
            // Điều kiện `is not AppException` rất quan trọng: ServiceUnavailableException
            // (503 do gọi service khác thất bại) là AppException, không phải bug -
            // nếu lọc chỉ theo status >= 500 thì stack trace kèm đường dẫn file
            // và tên service nội bộ sẽ bị lộ ra ngoài.
            //
            // Chỉ lỗi hệ thống thật sự (500 không phải AppException) mới thêm chi tiết.
            if (env.IsDevelopment() && statusCode >= 500 && exception is not AppException)
            {
                errors.Add($"Exception: {exception.GetType().Name} - {exception.Message}");
                if (exception.InnerException != null)
                {
                    errors.Add($"InnerException: {exception.InnerException.Message}");
                }
                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    errors.Add($"StackTrace: {exception.StackTrace}");
                }
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = ApiResponse.FailResult(message, errors);
            response.TraceId = context.TraceIdentifier;

            // Dùng WriteAsJsonAsync để đi qua cùng output formatter của MVC:
            // không lệch format (WriteIndented) và không cần tự tạo JsonSerializerOptions.
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}