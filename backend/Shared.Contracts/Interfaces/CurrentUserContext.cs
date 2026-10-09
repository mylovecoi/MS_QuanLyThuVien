using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Shared.Contracts.Interfaces
{
    /// <summary>
    /// Đọc thông tin người dùng hiện tại từ JWT token thông qua IHttpContextAccessor.
    /// Đăng ký với lifetime Scoped (theo request HTTP).
    /// </summary>
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null) return null;

                // Thử lấy từ "sub" claim (JWT chuẩn) trước, sau đó fallback sang NameIdentifier
                var value = user.FindFirstValue("sub")
                            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
