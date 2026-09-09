using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProfileService.Business.Dtos;
using ProfileService.Business.Exceptions;
using ProfileService.Business.Mappers;
using ProfileService.Business.Services.Interfaces;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;

namespace ProfileService.Business.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IPhongBanRepository _phongBanRepository;
        private readonly IChucVuRepository _chucVuRepository;

        public UserProfileService(
            IUserProfileRepository userProfileRepository,
            IPhongBanRepository phongBanRepository,
            IChucVuRepository chucVuRepository)
        {
            _userProfileRepository = userProfileRepository;
            _phongBanRepository = phongBanRepository;
            _chucVuRepository = chucVuRepository;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync()
        {
            var profiles = await _userProfileRepository.GetAllAsync();
            return profiles.Select(ProfileMapper.ToDto);
        }

        public async Task<UserProfileDto?> GetProfileByIdAsync(Guid id)
        {
            var profile = await _userProfileRepository.GetByIdAsync(id);
            return profile == null ? null : ProfileMapper.ToDto(profile);
        }

        public async Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
            return profile == null ? null : ProfileMapper.ToDto(profile);
        }

        public async Task<UserProfileDto> CreateProfileAsync(CreateUserProfileRequest request)
        {
            var existingProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
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

            var entity = new UserProfileEntity
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

            await _userProfileRepository.AddAsync(entity);
            await _userProfileRepository.SaveChangesAsync();

            return ProfileMapper.ToDto(entity);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
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

            _userProfileRepository.Update(profile);
            await _userProfileRepository.SaveChangesAsync();

            return ProfileMapper.ToDto(profile);
        }

        public async Task<bool> DeleteProfileAsync(Guid id)
        {
            var profile = await _userProfileRepository.GetByIdAsync(id);
            if (profile == null) return false;

            _userProfileRepository.Delete(profile);
            return await _userProfileRepository.SaveChangesAsync();
        }
    }
}
