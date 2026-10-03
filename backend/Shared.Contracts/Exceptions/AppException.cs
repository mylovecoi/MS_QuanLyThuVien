using System;
using System.Collections.Generic;

namespace Shared.Contracts.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public List<string>? Errors { get; }

        public AppException(string message, int statusCode = 400, List<string>? errors = null) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message, List<string>? errors = null) : base(message, 404, errors)
        {
        }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message, List<string>? errors = null) : base(message, 400, errors)
        {
        }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Không có quyền truy cập.", List<string>? errors = null) : base(message, 401, errors)
        {
        }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Bạn không có quyền truy cập tài nguyên này.", List<string>? errors = null) : base(message, 403, errors)
        {
        }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message, List<string>? errors = null) : base(message, 409, errors)
        {
        }
    }

    /// <summary>
    /// Lỗi hạ tầng khi gọi service khác (connection refused, timeout, 5xx).
    /// Tách riêng khỏi lỗi nghiệp vụ để AuthService rollback đăng ký
    /// và trả thông báo phù hợp thay vì báo "dữ liệu không hợp lệ".
    /// </summary>
    public class ServiceUnavailableException : AppException
    {
        public ServiceUnavailableException(string message, List<string>? errors = null) : base(message, 503, errors)
        {
        }
    }
}