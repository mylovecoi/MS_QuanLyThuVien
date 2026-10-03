using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Responses;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Mappers;
using ProfileService.Business.Services.Interfaces;
using ProfileService.Business.Validators;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;
using Shared.Contracts.Exceptions;

namespace ProfileService.Business.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPhongBanRepository _phongBanRepository;
        private readonly IChucVuRepository _chucVuRepository;
        private readonly IValidator<ProfileRequest> _validator;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(
            IProfileRepository profileRepository,
            IPhongBanRepository phongBanRepository,
            IChucVuRepository chucVuRepository,
            IValidator<ProfileRequest> validator,
            ILogger<ProfileService> logger)
        {
            _profileRepository = profileRepository;
            _phongBanRepository = phongBanRepository;
            _chucVuRepository = chucVuRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<PagedResult<ProfileDto>> GetProfilesAsync(int page, int pageSize)
        {
            // Paging ở tầng DB rồi mới map sang DTO, tránh tải toàn bộ bảng lên memory.
            var paged = await _profileRepository.GetPagedAsync(page, pageSize);
            var items = paged.ItemsOf<ProfileEntity>()
                .Select(ProfileMapper.ToDto)
                .ToList();

            return PagedResult<ProfileDto>.Create(items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        public async Task<ProfileDto?> GetProfileByIdAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            return profile == null ? null : ProfileMapper.ToDto(profile);
        }

        /// <summary>
        /// Lấy hồ sơ theo UserId, tự tạo hồ sơ rỗng nếu chưa có.
        ///
        /// Trước đây hàm này trả null khi chưa có hồ sơ và controller ném 404.
        /// Vấn đề là không phải mọi user đều đi qua luồng đăng ký (user seed sẵn,
        /// user import sau này) nên 404 là trạng thái bình thường chứ không phải
        /// lỗi, còn client lại phải tự xử lý. Nay tự tạo để invariant
        /// "1 User luôn có 1 Profile" được giữ.
        ///
        /// Lưu ý: hồ sơ tự tạo để trống vì ProfileService không có dữ liệu
        /// tên/email của user (thuộc IdentityService, cũng là DB khác server).
        /// Client sẽ điền và lưu lại qua PUT by-user.
        /// </summary>
        public async Task<ProfileDto> GetProfileByUserIdAsync(Guid userId)
        {
            var (profile, created) = await _profileRepository.EnsureExistsAsync(userId);

            if (created)
            {
                _logger.LogInformation(
                    "Chưa có hồ sơ cho UserId={UserId}, đã tạo hồ sơ rỗng để tự lành.",
                    userId);
            }

            return ProfileMapper.ToDto(profile);
        }

        public async Task<ProfileDto> CreateProfileAsync(ProfileRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_validator, request);

            var existingProfile = await _profileRepository.GetByUserIdAsync(request.UserId);
            if (existingProfile != null)
            {
                throw new ConflictException($"Hồ sơ người dùng với UserId '{request.UserId}' đã tồn tại.");
            }

            string tenPhongBan = string.Empty;
            if (request.PhongBanId.HasValue)
            {
                var phongBan = await _phongBanRepository.GetByIdAsync(request.PhongBanId.Value);
                if (phongBan != null) tenPhongBan = phongBan.TenPhongBan;
            }

            string tenChucVu = string.Empty;
            if (request.ChucVuId.HasValue)
            {
                var chucVu = await _chucVuRepository.GetByIdAsync(request.ChucVuId.Value);
                if (chucVu != null) tenChucVu = chucVu.TenChucVu;
            }

            var entity = new ProfileEntity
            {
                UserId = request.UserId,
                MaCanBo = request.MaCanBo,
                FullName = request.FullName,
                Cccd = request.Cccd,
                NgaySinh = request.NgaySinh,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                CoQuanPhoneNumber = request.CoQuanPhoneNumber,
                WorkEmail = request.WorkEmail,
                AvatarUrl = request.AvatarUrl,
                DiaChiHienNay = request.DiaChiHienNay,
                PhongBanId = request.PhongBanId,
                TenPhongBan = tenPhongBan,
                ChucVuId = request.ChucVuId,
                TenChucVu = tenChucVu
            };

            await _profileRepository.AddAsync(entity);
            await _profileRepository.SaveChangesAsync();

            return ProfileMapper.ToDto(entity);
        }

        public async Task<ProfileDto> UpdateProfileAsync(Guid userId, ProfileRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_validator, request);

            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new NotFoundException($"Không tìm thấy hồ sơ người dùng với UserId '{userId}'.");
            }

            if (request.PhongBanId.HasValue && request.PhongBanId != profile.PhongBanId)
            {
                var phongBan = await _phongBanRepository.GetByIdAsync(request.PhongBanId.Value);
                profile.PhongBanId = request.PhongBanId;
                profile.TenPhongBan = phongBan?.TenPhongBan ?? string.Empty;
            }

            if (request.ChucVuId.HasValue && request.ChucVuId != profile.ChucVuId)
            {
                var chucVu = await _chucVuRepository.GetByIdAsync(request.ChucVuId.Value);
                profile.ChucVuId = request.ChucVuId;
                profile.TenChucVu = chucVu?.TenChucVu ?? string.Empty;
            }

            profile.MaCanBo = request.MaCanBo;
            profile.FullName = request.FullName;
            profile.Cccd = request.Cccd;
            profile.NgaySinh = request.NgaySinh;
            profile.Gender = request.Gender;
            profile.PhoneNumber = request.PhoneNumber;
            profile.CoQuanPhoneNumber = request.CoQuanPhoneNumber;
            profile.WorkEmail = request.WorkEmail;
            profile.AvatarUrl = request.AvatarUrl;
            profile.DiaChiHienNay = request.DiaChiHienNay;

            _profileRepository.Update(profile);
            await _profileRepository.SaveChangesAsync();

            return ProfileMapper.ToDto(profile);
        }

        public async Task DeleteProfileAsync(Guid id)
        {
            // Ném NotFoundException thay vì trả bool: để controller không phải
            // tự dựng response lỗi, mọi lỗi đi chung 1 đường qua ExceptionMiddleware.
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                throw new NotFoundException($"Không tìm thấy hồ sơ người dùng với Id '{id}'.");
            }

            _profileRepository.Delete(profile);
            await _profileRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Xoá hồ sơ theo UserId, trả về false nếu không có hồ sơ nào.
        /// Dùng cho endpoint nội bộ: IdentityService gọi để hoàn tác hồ sơ
        /// đã tạo khi lưu user thất bại.
        /// </summary>
        public async Task<bool> DeleteProfileByUserIdAsync(Guid userId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return false;
            }

            _profileRepository.Delete(profile);
            await _profileRepository.SaveChangesAsync();
            return true;
        }
    }
}