using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using IdentityService.Business.Clients;
using IdentityService.Business.Constants;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using IdentityService.Business.Helpers;
using IdentityService.Business.Mappers;
using IdentityService.Business.Services.Interfaces;
using IdentityService.Business.Validators;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Enums;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Interfaces;
using Shared.Contracts.Responses;

namespace IdentityService.Business.Services.Implementations
{
    public class UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IProfileServiceClient profileServiceClient,
        ICurrentUserContext currentUserContext,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator,
        IValidator<ResetPasswordRequest> resetPasswordValidator,
        ILogger<UserService> logger) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IProfileServiceClient _profileServiceClient = profileServiceClient;
        private readonly ICurrentUserContext _currentUserContext = currentUserContext;
        private readonly IValidator<CreateUserRequest> _createValidator = createValidator;
        private readonly IValidator<UpdateUserRequest> _updateValidator = updateValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator = resetPasswordValidator;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<PagedResult<UserDetailResponse>> GetUsersAsync(int page, int pageSize, string? search)
        {
            var paged = await _userRepository.GetPagedWithRolesAsync(page, pageSize, search);
            var items = paged.ItemsOf<UserEntity>()
                .Select(u => u.ToDetailResponse())
                .ToList();

            return PagedResult<UserDetailResponse>.Create(items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        public async Task<UserDetailResponse> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetWithRolesByIdAsync(id)
                       ?? throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);

            return user.ToDetailResponse();
        }

        public async Task<UserDetailResponse> CreateUserAsync(CreateUserRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_createValidator, request);

            var username = request.Username.Trim();
            var email = request.Email.Trim();

            if (await _userRepository.GetByUsernameAsync(username) != null)
            {
                throw new ConflictException(ApplicationConstants.ErrorMessages.UsernameAlreadyExists);
            }

            if (await _userRepository.GetByEmailAsync(email) != null)
            {
                throw new ConflictException(ApplicationConstants.ErrorMessages.EmailAlreadyExists);
            }

            var roles = await EnsureRolesExistAsync(request.RoleIds);

            var newUser = new UserEntity
            {
                Username = username,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                FullName = request.FullName.Trim(),
                Gender = request.Gender,
                Status = request.Status,
                Role = roles.FirstOrDefault()?.Code ?? ApplicationConstants.Roles.User
            };

            foreach (var role in roles)
            {
                newUser.UserRoles.Add(new UserRoleEntity
                {
                    UserId = newUser.Id,
                    RoleId = role.Id
                });
            }

            // Giống luồng đăng ký: tạo hồ sơ TRƯỚC khi lưu user để tránh tài khoản
            // không có hồ sơ. Đảo thứ tự thì lỗi chỉ để lại hồ sơ mồ côi, không chặn
            // ai đăng ký lại (username/email có unique index không lọc IsDeleted).
            await _profileServiceClient.CreateProfileAsync(new InternalCreateProfileRequest
            {
                UserId = newUser.Id,
                FullName = newUser.FullName,
                WorkEmail = newUser.Email
            });

            try
            {
                await _userRepository.AddAsync(newUser);
                await _userRepository.SaveChangesAsync();
            }
            catch
            {
                await TryCompensateProfileAsync(newUser.Id);
                throw;
            }

            return await GetUserByIdAsync(newUser.Id);
        }

        public async Task<UserDetailResponse> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_updateValidator, request);

            var user = await _userRepository.GetWithRolesByIdAsync(id)
                       ?? throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);

            var email = request.Email.Trim();
            var conflict = await _userRepository.GetByEmailAsync(email);
            if (conflict != null && conflict.Id != id)
            {
                throw new ConflictException(ApplicationConstants.ErrorMessages.EmailAlreadyExists);
            }

            var roles = await EnsureRolesExistAsync(request.RoleIds);

            user.Email = email;
            user.FullName = request.FullName.Trim();
            user.Gender = request.Gender;
            user.Role = roles.FirstOrDefault()?.Code ?? user.Role;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            await _userRepository.ReplaceRolesAsync(id, roles.Select(r => r.Id).ToList());
            await _userRepository.SaveChangesAsync();

            return await GetUserByIdAsync(id);
        }

        public async Task<UserDetailResponse> UpdateStatusAsync(Guid id, ChangeUserStatusRequest request)
        {
            var user = await _userRepository.GetWithRolesByIdAsync(id)
                       ?? throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);

            // Không cho admin tự khóa chính mình để tránh tự đẩy mình ra khỏi hệ thống.
            if (id == _currentUserContext.UserId && request.Status != AccountStatus.Active)
            {
                throw new ConflictException("Bạn không thể khóa tài khoản của chính mình.");
            }

            user.Status = request.Status;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return await GetUserByIdAsync(id);
        }

        public async Task ResetPasswordAsync(Guid id, ResetPasswordRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_resetPasswordValidator, request);

            var user = await _userRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);

            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                       ?? throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);

            if (id == _currentUserContext.UserId)
            {
                throw new ConflictException("Bạn không thể xóa tài khoản của chính mình.");
            }

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Bỏ Id trùng/rỗng, bảo đảm mọi vai trò đều tồn tại, trả về entity tương ứng.
        /// </summary>
        private async Task<List<RoleEntity>> EnsureRolesExistAsync(IEnumerable<Guid>? roleIds)
        {
            var requested = (roleIds ?? Array.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (requested.Count == 0)
            {
                return new List<RoleEntity>();
            }

            var found = (await _roleRepository.GetByIdsAsync(requested)).ToList();
            var foundIds = found.Select(r => r.Id).ToHashSet();

            var missing = requested.Where(id => !foundIds.Contains(id)).ToList();
            if (missing.Count > 0)
            {
                throw new BadRequestException(
                    "Danh sách vai trò chứa vai trò không tồn tại.",
                    missing.Select(id => $"Vai trò không tồn tại: {id}").ToList());
            }

            return found;
        }

        private async Task TryCompensateProfileAsync(Guid userId)
        {
            try
            {
                await _profileServiceClient.DeleteProfileAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Không hoàn tác được hồ sơ UserId={UserId} sau khi tạo user thất bại. Cần xoá thủ công.",
                    userId);
            }
        }
    }
}
