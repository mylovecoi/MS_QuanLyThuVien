using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Exceptions;
using ProfileService.Business.Mappers;
using ProfileService.Business.Services.Interfaces;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;

namespace ProfileService.Business.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IPhongBanRepository _phongBanRepository;
        private readonly IChucVuRepository _chucVuRepository;

        public ProfileService(
            IProfileRepository profileRepository,
            IPhongBanRepository phongBanRepository,
            IChucVuRepository chucVuRepository)
        {
            _profileRepository = profileRepository;
            _phongBanRepository = phongBanRepository;
            _chucVuRepository = chucVuRepository;
        }

        public async Task<IEnumerable<ProfileDto>> GetAllProfilesAsync()
        {
            var profiles = await _profileRepository.GetAllAsync();
            return profiles.Select(ProfileMapper.ToDto);
        }

        public async Task<ProfileDto?> GetProfileByIdAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            return profile == null ? null : ProfileMapper.ToDto(profile);
        }

        public async Task<ProfileDto?> GetProfileByUserIdAsync(Guid userId)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            return profile == null ? null : ProfileMapper.ToDto(profile);
        }

        public async Task<ProfileDto> CreateProfileAsync(ProfileRequest request)
        {
            var existingProfile = await _profileRepository.GetByUserIdAsync(request.UserId);
            if (existingProfile != null)
            {
                throw new BadRequestException($"Hồ sơ người dùng với UserId '{request.UserId}' đã tồn tại.");
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

        public async Task<bool> DeleteProfileAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null) return false;

            _profileRepository.Delete(profile);
            return await _profileRepository.SaveChangesAsync();
        }
    }
}
