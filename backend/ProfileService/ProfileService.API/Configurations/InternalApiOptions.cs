namespace ProfileService.API.Configurations
{
    public class InternalApiOptions
    {
        public const string SectionName = "InternalApi";

        /// <summary>
        /// Khoá dùng chung giữa các service để xác thực request nội bộ
        /// (service-to-service), thay cho JWT vốn gắn với user.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
