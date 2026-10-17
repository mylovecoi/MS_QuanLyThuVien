using System;
using System.Collections.Generic;

namespace IdentityService.Business.Dtos.Responses
{
    /// <summary>
    /// Dữ liệu tài khoản cho trang quản lý của admin.
    /// Gender/Status giữ dạng số nguyên của enum để client tự ánh xạ nhãn.
    /// </summary>
    public class UserDetailResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int Gender { get; set; }
        public int Status { get; set; }
        public List<UserRoleResponse> Roles { get; set; } = new List<UserRoleResponse>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
