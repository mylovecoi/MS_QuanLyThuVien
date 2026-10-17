using FluentValidation;
using IdentityService.Business.Dtos.Requests;

namespace IdentityService.Business.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword)
                .Matches(@"[A-Za-z]")
                .WithMessage("Mật khẩu phải có ít nhất một chữ cái.")
                .Matches(@"\d")
                .WithMessage("Mật khẩu phải có ít nhất một chữ số.");
        }
    }
}
