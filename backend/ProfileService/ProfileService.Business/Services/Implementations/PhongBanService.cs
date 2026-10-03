using System.Linq;
using FluentValidation;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Mappers;
using ProfileService.Business.Services.Interfaces;
using ProfileService.Business.Validators;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Responses;

namespace ProfileService.Business.Services.Implementations
{
    public class PhongBanService : IPhongBanService
    {
        private readonly IPhongBanRepository _phongBanRepository;
        private readonly IValidator<PhongBanRequest> _validator;
        private readonly IValidator<ChucVuRequest> _chucVuValidator;

        public PhongBanService(
            IPhongBanRepository phongBanRepository,
            IValidator<PhongBanRequest> validator,
            IValidator<ChucVuRequest> chucVuValidator)
        {
            _phongBanRepository = phongBanRepository;
            _validator = validator;
            _chucVuValidator = chucVuValidator;
        }

        public async Task<PagedResult<PhongBanDto>> GetPhongBansAsync(int page, int pageSize)
        {
            var paged = await _phongBanRepository.GetPagedAsync(page, pageSize);
            var items = paged.ItemsOf<PhongBanEntity>()
                .Select(ProfileMapper.ToDto)
                .ToList();

            return PagedResult<PhongBanDto>.Create(items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        public async Task<PhongBanDto?> GetPhongBanByIdAsync(Guid id)
        {
            var item = await _phongBanRepository.GetByIdAsync(id);
            return item == null ? null : ProfileMapper.ToDto(item);
        }

        public async Task<PhongBanDto> CreatePhongBanAsync(PhongBanRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_validator, request);

            var existing = await _phongBanRepository.GetByMaPhongBanAsync(request.MaPhongBan);
            if (existing != null)
            {
                throw new ConflictException($"Mã phòng ban '{request.MaPhongBan}' đã tồn tại.");
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
        private readonly IValidator<ChucVuRequest> _chucVuValidator;

        public ChucVuService(IChucVuRepository chucVuRepository, IValidator<ChucVuRequest> chucVuValidator)
        {
            _chucVuRepository = chucVuRepository;
            _chucVuValidator = chucVuValidator;
        }

        public async Task<PagedResult<ChucVuDto>> GetChucVusAsync(int page, int pageSize)
        {
            var paged = await _chucVuRepository.GetPagedAsync(page, pageSize);
            var items = paged.ItemsOf<ChucVuEntity>()
                .Select(ProfileMapper.ToDto)
                .ToList();

            return PagedResult<ChucVuDto>.Create(items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        public async Task<ChucVuDto?> GetChucVuByIdAsync(Guid id)
        {
            var item = await _chucVuRepository.GetByIdAsync(id);
            return item == null ? null : ProfileMapper.ToDto(item);
        }

        public async Task<ChucVuDto> CreateChucVuAsync(ChucVuRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_chucVuValidator, request);

            var existing = await _chucVuRepository.GetByMaChucVuAsync(request.MaChucVu);
            if (existing != null)
            {
                throw new ConflictException($"Mã chức vụ '{request.MaChucVu}' đã tồn tại.");
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
