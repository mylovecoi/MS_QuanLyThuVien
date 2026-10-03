using Microsoft.Extensions.Configuration;

namespace Shared.Contracts.Configuration
{
    /// <summary>
    /// Fail-fast khi cấu hình bí mật còn thiếu hoặc còn là giá trị placeholder.
    /// Mục đích: tránh app khởi động với khoá rỗng/khoá yếu rồi mới phát hiện lúc chạy thật.
    /// </summary>
    public static class ConfigurationGuard
    {
        public static IConfiguration RequireSecrets(this IConfiguration configuration, params string[] keys)
        {
            if (keys is null || keys.Length == 0)
            {
                return configuration;
            }

            var problems = new List<string>();

            foreach (var key in keys)
            {
                var value = configuration[key];

                if (string.IsNullOrWhiteSpace(value))
                {
                    problems.Add($"{key} (thiếu hoặc rỗng)");
                }
                else if (IsPlaceholder(value))
                {
                    problems.Add($"{key} (vẫn là giá trị mẫu)");
                }
            }

            if (problems.Count > 0)
            {
                throw new InvalidOperationException(
                    "Cấu hình bí mật chưa hợp lệ: " + string.Join("; ", problems) + ". " +
                    "Hãy cấu hình qua user-secrets hoặc biến môi trường, ví dụ: " +
                    "dotnet user-secrets set \"<key>\" \"<value>\". " +
                    "Xem appsettings.Example.json để biết các key bắt buộc.");
            }

            return configuration;
        }

        private static bool IsPlaceholder(string value) =>
            value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase)
            || value.Contains("CHANGEME", StringComparison.OrdinalIgnoreCase)
            || value.Contains("<<", StringComparison.Ordinal)
            || value.Contains(">>", StringComparison.Ordinal);
    }
}