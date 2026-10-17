using System;

namespace IdentityService.Business.Dtos.Responses
{
    /// <summary>
    /// Thông tin vai trò gọn, dùng trong UserDetailResponse.
    /// </summary>
    public class UserRoleResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
