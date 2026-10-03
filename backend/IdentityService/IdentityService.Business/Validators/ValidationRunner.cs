using FluentValidation;
using Shared.Contracts.Exceptions;

namespace IdentityService.Business.Validators
{
    /// <summary>
    /// Chạy validator của FluentValidation rồi chuyển lỗi thành
    /// <see cref="BadRequestException"/> để middleware trả về 400 kèm errors[].
    /// DataAnnotations đã lo phần ràng buộc định dạng (độ dài, email, required),
    /// nên validator ở đây chỉ bổ sung rule nghiệp vụ cần tra cứu thêm.
    /// </summary>
    public static class ValidationRunner
    {
        public static async Task EnsureValidAsync<T>(IValidator<T> validator, T instance)
        {
            var result = await validator.ValidateAsync(instance);

            if (result.IsValid)
            {
                return;
            }

            var errors = result.Errors
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct()
                .ToList();

            throw new BadRequestException("Dữ liệu không hợp lệ.", errors);
        }
    }
}