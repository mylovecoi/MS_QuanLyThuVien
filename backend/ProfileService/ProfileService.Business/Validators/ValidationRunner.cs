using FluentValidation;
using Shared.Contracts.Exceptions;

namespace ProfileService.Business.Validators
{
    /// <summary>
    /// Chạy validator của FluentValidation rồi chuyển lỗi thành
    /// <see cref="BadRequestException"/> để middleware trả 400 kèm errors[].
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