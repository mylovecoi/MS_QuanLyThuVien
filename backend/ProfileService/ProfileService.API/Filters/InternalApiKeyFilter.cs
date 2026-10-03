using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ProfileService.API.Configurations;
using ProfileService.Business.Dtos.Responses;

namespace ProfileService.API.Filters
{
    /// <summary>
    /// Chặn endpoint nội bộ (service-to-service): chỉ chấp nhận request
    /// mang đúng X-Internal-Api-Key. Thay thế việc cấp JWT cho từng service.
    /// </summary>
    public class InternalApiKeyFilter : IAsyncAuthorizationFilter
    {
        public const string HeaderName = "X-Internal-Api-Key";

        private readonly InternalApiOptions _options;

        public InternalApiKeyFilter(IOptions<InternalApiOptions> options)
        {
            _options = options.Value;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string? message = null;
            var statusCode = StatusCodes.Status200OK;

            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                // Chưa cấu hình khoá -> chặn toàn bộ thay vì mở cửa cho tất cả.
                message = "Internal API chưa được cấu hình (InternalApi:ApiKey).";
                statusCode = StatusCodes.Status503ServiceUnavailable;
            }
            else if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided)
                || !string.Equals(provided.ToString(), _options.ApiKey, StringComparison.Ordinal))
            {
                message = "Thiếu hoặc sai khoá Internal API.";
                statusCode = StatusCodes.Status401Unauthorized;
            }

            if (message != null)
            {
                // Authorization filter short-circuit nên ApiResponseFilter
                // (IActionFilter) không chạy -> phải tự dựng envelope ở đây.
                var response = ApiResponse.FailResult(message);
                response.TraceId = context.HttpContext.TraceIdentifier;

                context.Result = new ObjectResult(response) { StatusCode = statusCode };
            }

            return Task.CompletedTask;
        }
    }
}