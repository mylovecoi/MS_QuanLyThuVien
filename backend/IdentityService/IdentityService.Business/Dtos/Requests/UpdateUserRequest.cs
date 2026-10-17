using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityService.DataAccess.Enums;

namespace IdentityService.Business.Dtos.Requests
{
    /// <summary>
    /// Admin cập nhật thông tin tài khoản. Không cho đổi Username hay mật khẩu
    /// (mật khẩu có endpoint riêng, trạng thái có endpoint riêng).
    /// </summary>
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string FullName { get; set; } = string.Empty;

        public Gender Gender { get; set; } = Gender.Unknown;

        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
