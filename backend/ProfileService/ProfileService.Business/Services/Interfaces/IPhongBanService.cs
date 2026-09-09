using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.Business.Dtos;

namespace ProfileService.Business.Services.Interfaces
{
    public interface IPhongBanService
    {
        Task<IEnumerable<PhongBanDto>> GetAllPhongBansAsync();
        Task<PhongBanDto?> GetPhongBanByIdAsync(Guid id);
        Task<PhongBanDto> CreatePhongBanAsync(CreatePhongBanRequest request);
    }

    public interface IChucVuService
    {
        Task<IEnumerable<ChucVuDto>> GetAllChucVusAsync();
        Task<ChucVuDto?> GetChucVuByIdAsync(Guid id);
        Task<ChucVuDto> CreateChucVuAsync(CreateChucVuRequest request);
    }
}
