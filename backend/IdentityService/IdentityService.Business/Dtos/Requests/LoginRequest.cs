using System.ComponentModel.DataAnnotations;

namespace IdentityService.Business.Dtos.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; } = string.Empty;
    }
}
