using System;
using System.Collections.Generic;

namespace Shared.Contracts.Responses
{
    /// <summary>
    /// Cho phép filter/middleware gán TraceId vào envelope mà không cần
    /// biết trước kiểu generic cụ thể.
    /// </summary>
    public interface IApiResponse
    {
        string? TraceId { get; set; }
    }

    /// <summary>
    /// Envelope chuẩn cho mọi response của API.
    /// Không chứa StatusCode vì HTTP status code đã nằm trên header.
    /// </summary>
    public class ApiResponse<T> : IApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResult(T? data, string? message = "Thao tác thành công.")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> FailResult(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Bản không generic cho các nơi không biết trước kiểu payload
    /// (model validation, exception middleware).
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static new ApiResponse SuccessResult(object? data = null, string? message = "Thao tác thành công.")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static new ApiResponse FailResult(string message, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}