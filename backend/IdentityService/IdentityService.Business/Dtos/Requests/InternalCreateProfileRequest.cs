namespace IdentityService.Business.Dtos.Requests
{
    /// <summary>
    /// DTO dùng nội bộ để gọi sang ProfileService tạo profile khi register user mới.
    /// Chỉ chứa các field cơ bản; các field khác (MaCanBo, PhoneNumber...) do admin cập nhật sau.
    /// </summary>
    public class InternalCreateProfileRequest
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
    }
}
