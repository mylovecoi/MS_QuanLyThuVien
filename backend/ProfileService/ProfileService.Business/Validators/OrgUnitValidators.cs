using FluentValidation;
using ProfileService.Business.Dtos.Requests;

namespace ProfileService.Business.Validators
{
    public class PhongBanRequestValidator : AbstractValidator<PhongBanRequest>
    {
        public PhongBanRequestValidator()
        {
            RuleFor(x => x.MaPhongBan)
                .NotEmpty().WithMessage("Mã phòng ban không được để trống.")
                .MaximumLength(50).WithMessage("Mã phòng ban không được vượt quá 50 ký tự.")
                .Matches("^[A-Za-z0-9._-]+$")
                .WithMessage("Mã phòng ban chỉ được chứa chữ, số và các ký tự . _ -");

            RuleFor(x => x.TenPhongBan)
                .NotEmpty().WithMessage("Tên phòng ban không được để trống.")
                .MaximumLength(200).WithMessage("Tên phòng ban không được vượt quá 200 ký tự.");

            RuleFor(x => x.OrderNo)
                .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị không được nhỏ hơn 0.");
        }
    }

    public class ChucVuRequestValidator : AbstractValidator<ChucVuRequest>
    {
        public ChucVuRequestValidator()
        {
            RuleFor(x => x.MaChucVu)
                .NotEmpty().WithMessage("Mã chức vụ không được để trống.")
                .MaximumLength(50).WithMessage("Mã chức vụ không được vượt quá 50 ký tự.")
                .Matches("^[A-Za-z0-9._-]+$")
                .WithMessage("Mã chức vụ chỉ được chứa chữ, số và các ký tự . _ -");

            RuleFor(x => x.TenChucVu)
                .NotEmpty().WithMessage("Tên chức vụ không được để trống.")
                .MaximumLength(200).WithMessage("Tên chức vụ không được vượt quá 200 ký tự.");

            RuleFor(x => x.CapBac)
                .InclusiveBetween(0, 10).WithMessage("Cấp bậc phải nằm trong khoảng từ 0 đến 10.");
        }
    }
}