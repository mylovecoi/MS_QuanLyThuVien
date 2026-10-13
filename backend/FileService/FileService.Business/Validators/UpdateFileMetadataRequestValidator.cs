using FileService.Business.Dtos.Requests;
using FluentValidation;

namespace FileService.Business.Validators
{
    public class UpdateFileMetadataRequestValidator : AbstractValidator<UpdateFileMetadataRequest>
    {
        public UpdateFileMetadataRequestValidator()
        {
            RuleFor(x => x.FileName)
                .MaximumLength(255).WithMessage("Tên tệp tin không được vượt quá 255 ký tự.");

            RuleFor(x => x.MoTa)
                .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");
        }
    }
}
