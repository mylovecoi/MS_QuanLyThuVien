using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using IdentityService.Business.Dtos.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Exceptions;

namespace IdentityService.Business.Clients
{
    /// <summary>
    /// Client gọi ProfileService từ IdentityService (service-to-service).
    ///
    /// Có retry vì đăng ký là luồng nhạy cảm với thời điểm khởi động:
    /// nếu ProfileService chưa listen, lời gọi đầu tiên sẽ bị connection refused
    /// ngay cả khi service sẽ lên sau vài giây.
    /// </summary>
    public class ProfileServiceClient : IProfileServiceClient
    {
        internal const string InternalApiKeyHeaderName = "X-Internal-Api-Key";

        private const int MaxRetryDelaySeconds = 8;

        private readonly HttpClient _httpClient;
        private readonly ILogger<ProfileServiceClient> _logger;
        private readonly TimeSpan _perAttemptTimeout;
        private readonly int _maxRetries;
        private readonly TimeSpan _retryBaseDelay;

        public ProfileServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<ProfileServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            var section = configuration.GetSection("ProfileService");

            _perAttemptTimeout = TimeSpan.FromSeconds(ReadInt(section, "TimeoutSeconds", 10));
            _maxRetries = Math.Max(0, ReadInt(section, "MaxRetries", 4));
            _retryBaseDelay = TimeSpan.FromMilliseconds(ReadInt(section, "RetryBaseDelayMilliseconds", 500));

            // Timeout do HttpClient quản lý sẽ ném TaskCanceledException với thông
            // báo khó hiểu ("configured HttpClient.Timeout of X seconds elapsing")
            // và không cho phép phân biệt giữa timeout với hủy chủ động.
            // Đặt vô hạn rồi tự đặt ngân sách thời gian từng lần thử bằng
            // CancellationTokenSource để kiểm soát chính xác và log rõ ràng.
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;

            var apiKey = configuration["InternalApi:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(InternalApiKeyHeaderName, apiKey);
            }
            else
            {
                _logger.LogWarning(
                    "Chưa cấu hình InternalApi:ApiKey - mọi lệnh gọi ProfileService sẽ bị từ chối 401.");
            }
        }

        public async Task CreateProfileAsync(InternalCreateProfileRequest request)
        {
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    await SendCreateAsync(request);
                    return;
                }
                catch (Exception ex) when (IsTransient(ex) && attempt < _maxRetries)
                {
                    // Backoff luỹ thừa, chậm lại dần. Mục đích là "phủ" được thời gian
                    // ProfileService cần để khởi động (migrate + seed), chứ không
                    // phải bắn liên tục vào một service chưa sẵn sàng.
                    var delaySeconds = Math.Min(
                        MaxRetryDelaySeconds,
                        _retryBaseDelay.TotalSeconds * Math.Pow(2, attempt));

                    _logger.LogWarning(
                        "Gọi ProfileService thất bại (lần {Attempt}/{MaxAttempts}), thử lại sau {Delay:F1}s. Lý do: {Reason}",
                        attempt + 1, _maxRetries + 1, delaySeconds, Describe(ex));

                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Không tạo được hồ sơ cho UserId={UserId} sau {Attempts} lần thử. Lý do: {Reason}",
                        request.UserId, attempt + 1, Describe(ex));

                    throw ToServiceUnavailable(ex);
                }
            }
        }

        private async Task SendCreateAsync(InternalCreateProfileRequest request)
        {
            using var cts = new CancellationTokenSource(_perAttemptTimeout);
            using var response = await _httpClient.PostAsJsonAsync("api/internal/profiles", request, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "ProfileService từ chối tạo hồ sơ cho UserId={UserId}. Status={StatusCode}. Body={Body}",
                request.UserId, (int)response.StatusCode, body);

            throw new ServiceUnavailableException(
                $"Tạo hồ sơ thất bại: hệ thống dữ liệu người dùng trả về {(int)response.StatusCode} ({response.ReasonPhrase}). {Summarize(body)}");
        }

        public async Task DeleteProfileAsync(Guid userId)
        {
            try
            {
                using var cts = new CancellationTokenSource(_perAttemptTimeout);
                using var response = await _httpClient.DeleteAsync($"api/internal/profiles/by-user/{userId}", cts.Token);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                var body = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "ProfileService từ chối xoá hồ sơ UserId={UserId}. Status={StatusCode}. Body={Body}",
                    userId, (int)response.StatusCode, body);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                // Hàm này chỉ dùng để hoàn tác, nên nuốt lỗi ở đây là chấp nhận được:
                // caller vẫn ném lỗi gốc, còn việc dọn dẹp sẽ ghi log.
                _logger.LogWarning(ex, "Không gọi được ProfileService để xoá hồ sơ UserId={UserId}", userId);
            }
        }

        /// <summary>
        /// Lỗi tạm thời thì đáng thử lại: ProfileService chưa listen, hoặc quá tải.
        /// Lỗi nghiệp vụ (4xx) thì không: thử lại cũng ra kết quả y hệt.
        /// </summary>
        private static bool IsTransient(Exception ex) => ex switch
        {
            HttpRequestException => true,
            TaskCanceledException => true,
            OperationCanceledException => true,
            _ => false
        };

        private static ServiceUnavailableException ToServiceUnavailable(Exception ex) => ex switch
        {
            ServiceUnavailableException svc => svc,

            // OperationCanceledException do CancellationTokenSource bắn ra khi hết
            // thời gian chờ trong một lần thử.
            OperationCanceledException => new ServiceUnavailableException(
                "Không tạo được hồ sơ: hệ thống dữ liệu người dùng phản hồi quá chậm. Vui lòng thử lại sau."),

            HttpRequestException => new ServiceUnavailableException(
                "Không tạo được hồ sơ: không kết nối được tới hệ thống dữ liệu người dùng. Vui lòng thử lại sau."),

            _ => new ServiceUnavailableException("Không tạo được hồ sơ. Vui lòng thử lại sau.")
        };

        private static string Describe(Exception ex) => ex switch
        {
            HttpRequestException http => $"không kết nối được ({http.Message})",
            OperationCanceledException => "hết thời gian chờ",
            ServiceUnavailableException svc => svc.Message,
            _ => ex.Message
        };

        private static int ReadInt(IConfiguration section, string key, int fallback)
        {
            var raw = section[key];
            return int.TryParse(raw, out var value) && value > 0 ? value : fallback;
        }

        private static string Summarize(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return "(không có nội dung)";

            return body.Length > 500 ? body[..500] + "..." : body;
        }
    }
}
