using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Business.Dtos.Requests
{
    /// <summary>
    /// Dùng chung cho tạo mới và cập nhật vai trò.
    /// Với cập nhật, Id lấy từ route chứ không nhận trong body.
    /// </summary>
    public class RoleRequest
    {
        [Required(ErrorMessage = "Mã vai trò không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Mã vai trò phải từ 2 đến 50 ký tự.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên vai trò không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Mô tả không được vượt quá 250 ký tự.")]
        public string Description { get; set; } = string.Empty;

        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
}