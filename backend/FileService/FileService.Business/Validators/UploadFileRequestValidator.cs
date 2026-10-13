using FileService.Business.Dtos.Requests;
using FluentValidation;

namespace FileService.Business.Validators
{
    public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
    {
        public UploadFileRequestValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Tập tin tải lên không được để trống.")
                .Must(f => f != null && f.Length > 0).WithMessage("Tập tin tải lên không được có dung lượng bằng 0.");

            RuleFor(x => x.MoTa)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");
        }
    }
}
