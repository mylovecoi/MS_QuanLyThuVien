using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;

namespace ProfileService.Business.Services.Interfaces
{
    public interface IPhongBanService
    {
        Task<IEnumerable<PhongBanDto>> GetAllPhongBansAsync();
        Task<PhongBanDto?> GetPhongBanByIdAsync(Guid id);
        Task<PhongBanDto> CreatePhongBanAsync(PhongBanRequest request);
    }

    public interface IChucVuService
    {
        Task<IEnumerable<ChucVuDto>> GetAllChucVusAsync();
        Task<ChucVuDto?> GetChucVuByIdAsync(Guid id);
        Task<ChucVuDto> CreateChucVuAsync(ChucVuRequest request);
    }
}
