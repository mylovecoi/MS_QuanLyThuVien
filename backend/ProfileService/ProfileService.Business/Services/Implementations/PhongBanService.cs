using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Exceptions;
using ProfileService.Business.Mappers;
using ProfileService.Business.Services.Interfaces;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;

namespace ProfileService.Business.Services.Implementations
{
    public class PhongBanService : IPhongBanService
    {
        private readonly IPhongBanRepository _phongBanRepository;

        public PhongBanService(IPhongBanRepository phongBanRepository)
        {
            _phongBanRepository = phongBanRepository;
        }

        public async Task<IEnumerable<PhongBanDto>> GetAllPhongBansAsync()
        {
            var list = await _phongBanRepository.GetAllAsync();
            return list.Select(ProfileMapper.ToDto);
        }

        public async Task<PhongBanDto?> GetPhongBanByIdAsync(Guid id)
        {
            var item = await _phongBanRepository.GetByIdAsync(id);
            return item == null ? null : ProfileMapper.ToDto(item);
        }

        public async Task<PhongBanDto> CreatePhongBanAsync(PhongBanRequest request)
        {
            var existing = await _phongBanRepository.GetByMaPhongBanAsync(request.MaPhongBan);
            if (existing != null)
            {
                throw new BadRequestException($"Mã phòng ban '{request.MaPhongBan}' đã tồn tại.");
            }

            var entity = new PhongBanEntity
            {
                MaPhongBan = request.MaPhongBan,
                TenPhongBan = request.TenPhongBan,
                ParentId = request.ParentId,
                OrderNo = request.OrderNo
            };

            await _phongBanRepository.AddAsync(entity);
            await _phongBanRepository.SaveChangesAsync();

            return ProfileMapper.ToDto(entity);
        }
    }

    public class ChucVuService : IChucVuService
    {
        private readonly IChucVuRepository _chucVuRepository;

        public ChucVuService(IChucVuRepository chucVuRepository)
        {
            _chucVuRepository = chucVuRepository;
        }

        public async Task<IEnumerable<ChucVuDto>> GetAllChucVusAsync()
        {
            var list = await _chucVuRepository.GetAllAsync();
            return list.Select(ProfileMapper.ToDto);
        }

        public async Task<ChucVuDto?> GetChucVuByIdAsync(Guid id)
        {
            var item = await _chucVuRepository.GetByIdAsync(id);
            return item == null ? null : ProfileMapper.ToDto(item);
        }

        public async Task<ChucVuDto> CreateChucVuAsync(ChucVuRequest request)
        {
            var existing = await _chucVuRepository.GetByMaChucVuAsync(request.MaChucVu);
            if (existing != null)
            {
                throw new BadRequestException($"Mã chức vụ '{request.MaChucVu}' đã tồn tại.");
            }

            var entity = new ChucVuEntity
            {
                MaChucVu = request.MaChucVu,
                TenChucVu = request.TenChucVu,
                CapBac = request.CapBac
            };

            await _chucVuRepository.AddAsync(entity);
            await _chucVuRepository.SaveChangesAsync();

            return ProfileMapper.ToDto(entity);
        }
    }
}
