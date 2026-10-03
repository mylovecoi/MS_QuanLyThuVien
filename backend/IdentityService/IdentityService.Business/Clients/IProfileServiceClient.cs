using System;
using System.Threading.Tasks;
using IdentityService.Business.Dtos.Requests;
using Shared.Contracts.Exceptions;

namespace IdentityService.Business.Clients
{
    public interface IProfileServiceClient
    {
        /// <summary>
        /// Gọi sang ProfileService để tạo Profile mới khi user vừa được đăng ký.
        /// Ném <c>ServiceUnavailableException</c> nếu không tạo được.
        /// </summary>
        Task CreateProfileAsync(InternalCreateProfileRequest request);

        /// <summary>
        /// Hoàn tác hồ sơ đã tạo, dùng khi IdentityService lưu user thất bại.
        /// Idempotent - không có hồ sơ thì không coi là lỗi.
        /// </summary>
        Task DeleteProfileAsync(Guid userId);
    }
}
