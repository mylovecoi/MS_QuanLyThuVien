using System.ComponentModel.DataAnnotations;

namespace IdentityService.Business.Dtos.Requests
{
    /// <summary>
    /// Admin đặt lại mật khẩu cho một tài khoản.
    /// </summary>
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
