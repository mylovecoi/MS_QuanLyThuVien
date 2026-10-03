using FluentValidation;
using ProfileService.Business.Dtos.Requests;

namespace ProfileService.Business.Validators
{
    /// <summary>
    /// Luồng đăng ký tạo profile tối thiểu (FullName + WorkEmail, các trường khác rỗng),
    /// nên các trường bắt buộc ở đây chỉ áp dụng khi client gửi lên.
    /// </summary>
    public class ProfileRequestValidator : AbstractValidator<ProfileRequest>
    {
        public ProfileRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty)
                .WithMessage("UserId không được là giá trị rỗng.");

            RuleFor(x => x.FullName)
                .MaximumLength(200)
                .WithMessage("Họ và tên không được vượt quá 200 ký tự.");

            RuleFor(x => x.WorkEmail)
                .MaximumLength(256)
                .WithMessage("Email công việc không được vượt quá 256 ký tự.")
                .Must(e => string.IsNullOrWhiteSpace(e) || IsValidEmail(e))
                .WithMessage("Email công việc không hợp lệ.");

            RuleFor(x => x.MaCanBo)
                .MaximumLength(50)
                .WithMessage("Mã cán bộ không được vượt quá 50 ký tự.");

            RuleFor(x => x.Cccd)
                .MaximumLength(20)
                .WithMessage("CCCD không được vượt quá 20 ký tự.");

            RuleFor(x => x.NgaySinh)
                .Must(d => d == null || d.Value <= DateTime.UtcNow.AddYears(-10))
                .WithMessage("Ngày sinh không hợp lệ.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("Số điện thoại không được vượt quá 20 ký tự.");

            RuleFor(x => x.CoQuanPhoneNumber)
                .MaximumLength(20)
                .WithMessage("Số điện thoại cơ quan không được vượt quá 20 ký tự.");

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(500)
                .WithMessage("Đường dẫn ảnh đại diện không được vượt quá 500 ký tự.");
        }

        private static bool IsValidEmail(string email)
        {
            var trimmed = email.Trim();
            var atIndex = trimmed.IndexOf('@');
            if (atIndex <= 0 || atIndex == trimmed.Length - 1)
            {
                return false;
            }

            // Cho phép email dạng user@domain (không bắt buộc có TLD) vì dữ liệu
            // nội bộ có thể dùng tên miền nội bộ không có dấu chấm.
            return !trimmed.Contains(' ');
        }
    }
}