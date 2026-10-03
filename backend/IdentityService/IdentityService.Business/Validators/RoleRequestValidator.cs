using FluentValidation;
using IdentityService.Business.Dtos.Requests;

namespace IdentityService.Business.Validators
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Code)
                .Matches("^[A-Z][A-Z0-9_]*$")
                .WithMessage("Mã vai trò phải gồm chữ in hoa, số và dấu gạch dưới, bắt đầu bằng chữ cái.");

            RuleFor(x => x.Name)
                .Must(s => !string.IsNullOrWhiteSpace(s) && s == s.Trim())
                .WithMessage("Tên vai trò không được để trống hoặc có khoảng trắng thừa.");

            RuleFor(x => x.Description)
                .Must(s => s == null || s == s.Trim())
                .WithMessage("Mô tả không được có khoảng trắng thừa ở đầu hoặc cuối.");

            RuleFor(x => x.PermissionIds)
                .Must(ids => ids == null || ids.All(id => id != Guid.Empty))
                .WithMessage("Danh sách quyền hạn chứa giá trị không hợp lệ.");
        }
    }
}