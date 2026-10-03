using System;

namespace ProfileService.Business.Dtos.Requests
{
    /// <summary>
    /// DTO cho endpoint nội bộ (service-to-service).
    /// Chỉ nhận các trường IdentityService đã có sẵn lúc đăng ký;
    /// phần còn l của hồ sơ để trống cho user bổ sung sau.
    /// </summary>
    public class InternalCreateProfileRequest
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
    }
}
