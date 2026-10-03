using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using Shared.Contracts.Responses;

namespace ProfileService.Business.Services.Interfaces
{
    public interface IPhongBanService
    {
        Task<PagedResult<PhongBanDto>> GetPhongBansAsync(int page, int pageSize);
        Task<PhongBanDto?> GetPhongBanByIdAsync(Guid id);
        Task<PhongBanDto> CreatePhongBanAsync(PhongBanRequest request);
    }

    public interface IChucVuService
    {
        Task<PagedResult<ChucVuDto>> GetChucVusAsync(int page, int pageSize);
        Task<ChucVuDto?> GetChucVuByIdAsync(Guid id);
        Task<ChucVuDto> CreateChucVuAsync(ChucVuRequest request);
    }
}
