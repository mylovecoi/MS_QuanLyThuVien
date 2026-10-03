using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Responses;

namespace Shared.Contracts.Extensions
{
    /// <summary>
    /// Khối cấu hình trùng lặp giữa các service: envelope cho model validation
    /// và status-code pages cho response không có body (401/403/404/405).
    /// Gom vào đây để sửa một chỗ áp dụng cho mọi service.
    /// </summary>
    public static class ApiResponseConventions
    {
        /// <summary>
        /// ModelStateInvalidFilter short-circuit trước ApiResponseFilter nên filter
        /// không kịp gán traceId -> factory tự dựng envelope và gán traceId luôn.
        /// </summary>
        public static IServiceCollection AddApiResponseConventions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors.Select(x =>
                            string.IsNullOrEmpty(x.ErrorMessage)
                                ? x.Exception?.Message ?? "Lỗi không xác định"
                                : x.ErrorMessage))
                        .ToList();

                    var response = ApiResponse.FailResult("Dữ liệu đầu vào không hợp lệ.", errors);
                    response.TraceId = context.HttpContext.TraceIdentifier;

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }

        /// <summary>
        /// Phải đăng ký TRƯỚC UseAuthentication: JwtBearerHandler challenge sẽ
        /// short-circuit nên middleware phía dưới (kể cả status code pages)
        /// không chạy được. Đặt trước để 401/403 cũng có envelope ApiResponse.
        /// </summary>
        public static IApplicationBuilder UseApiResponseStatusCodePages(this IApplicationBuilder app)
        {
            return app.UseStatusCodePages(async statusCodeContext =>
            {
                var context = statusCodeContext.HttpContext;
                var message = context.Response.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Chưa xác thực. Vui lòng đăng nhập.",
                    StatusCodes.Status403Forbidden => "Bạn không có quyền truy cập tài nguyên này.",
                    StatusCodes.Status404NotFound => "Không tìm thấy tài nguyên yêu cầu.",
                    StatusCodes.Status405MethodNotAllowed => "Phương thức HTTP không được hỗ trợ.",
                    _ => "Yêu cầu không thành công."
                };

                var response = ApiResponse.FailResult(message);
                response.TraceId = context.TraceIdentifier;

                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsJsonAsync(response);
            });
        }
    }
}