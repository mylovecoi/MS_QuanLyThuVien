using FluentValidation;
using Shared.Contracts.Exceptions;

namespace FileService.Business.Validators
{
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
