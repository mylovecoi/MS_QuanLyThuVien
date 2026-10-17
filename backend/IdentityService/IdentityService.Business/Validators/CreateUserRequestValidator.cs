using FluentValidation;
using IdentityService.Business.Dtos.Requests;

namespace IdentityService.Business.Validators
{
    /// <summary>
    /// Rule nghiệp vụ khi admin tạo tài khoản. Ràng buộc định dạng (required,
    /// độ dài, email) đã do DataAnnotations trên <see cref="CreateUserRequest"/> lo.
    /// </summary>
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .Matches("^[a-zA-Z0-9._-]+$")
                .WithMessage("Tên đăng nhập chỉ được chứa chữ, số và các ký tự . _ -")
                .Must(u => !u.All(char.IsDigit))
                .WithMessage("Tên đăng nhập không được chỉ gồm chữ số.");

            RuleFor(x => x.Password)
                .Matches(@"[A-Za-z]")
                .WithMessage("Mật khẩu phải có ít nhất một chữ cái.")
                .Matches(@"\d")
                .WithMessage("Mật khẩu phải có ít nhất một chữ số.");

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
