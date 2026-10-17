using FluentValidation;
using IdentityService.Business.Dtos.Requests;

namespace IdentityService.Business.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .MaximumLength(256)
                .WithMessage("Email không được vượt quá 256 ký tự.");

            RuleFor(x => x.FullName)
                .Must(s => s == s.Trim())
                .WithMessage("Họ và tên không được có khoảng trắng thừa ở đầu hoặc cuối.")
                .MaximumLength(200)
                .WithMessage("Họ và tên không được vượt quá 200 ký tự.");

            RuleFor(x => x.RoleIds)
                .Must(ids => ids == null || ids.All(id => id != Guid.Empty))
                .WithMessage("Danh sách vai trò chứa giá trị không hợp lệ.");
        }
    }
}
