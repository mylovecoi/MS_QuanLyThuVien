using FluentValidation;
using IdentityService.Business.Dtos.Requests;

namespace IdentityService.Business.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
                .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                // Trần này chỉ chặn payload rác quá lớn, không thay đổi quy tắc mật khẩu.
                .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự.");
        }
    }
}