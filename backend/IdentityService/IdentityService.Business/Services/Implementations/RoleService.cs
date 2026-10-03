using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using IdentityService.Business.Mappers;
using IdentityService.Business.Services.Interfaces;
using IdentityService.Business.Validators;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Repositories.Interfaces;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Responses;

namespace IdentityService.Business.Services.Implementations
{
    public class RoleService : IRoleService
    {
        /// <summary>
        /// Vai trò hệ thống: luôn được sửa (tên, mô tả, quyền) nhưng không được đổi mã hay xóa.
        /// </summary>
        public const string AdminRoleCode = "ADMIN";

        private readonly IRoleRepository _roleRepository;
        private readonly IValidator<RoleRequest> _validator;

        public RoleService(IRoleRepository roleRepository, IValidator<RoleRequest> validator)
        {
            _roleRepository = roleRepository;
            _validator = validator;
        }

        public async Task<PagedResult<RoleResponse>> GetRolesAsync(int page, int pageSize)
        {
            // Paging áp dụng trên database rồi mới map sang DTO,
            // tránh tải toàn bộ bảng RolePermission lên memory.
            var paged = await _roleRepository.GetPagedWithPermissionsAsync(page, pageSize);
            var items = paged.ItemsOf<RoleEntity>()
                .Select(r => r.ToResponse())
                .ToList();

            return PagedResult<RoleResponse>.Create(items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        public async Task<RoleResponse> GetRoleByIdAsync(Guid id)
        {
            var role = await _roleRepository.GetWithPermissionsAsync(id);
            if (role == null)
            {
                throw new NotFoundException($"Không tìm thấy vai trò với Id '{id}'.");
            }

            return role.ToResponse();
        }

        public async Task<IReadOnlyList<PermissionResponse>> GetAllPermissionsAsync()
        {
            var permissions = await _roleRepository.GetAllPermissionsAsync();
            return permissions.Select(p => p.ToResponse()).ToList();
        }

        public async Task<RoleResponse> CreateRoleAsync(RoleRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_validator, request);

            var code = request.Code.Trim().ToUpperInvariant();

            if (code == AdminRoleCode)
            {
                throw new ConflictException("Vai trò ADMIN là vai trò hệ thống, không thể tạo thêm.");
            }

            if (await _roleRepository.GetByCodeAsync(code) != null)
            {
                throw new ConflictException($"Mã vai trò '{code}' đã tồn tại.");
            }

            var permissionIds = await EnsurePermissionsExistAsync(request.PermissionIds);

            var role = new RoleEntity
            {
                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty
            };

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();

            // Thay toàn bộ tập quyền sau khi role đã có Id để tạo đúng khóa ngoại.
            await _roleRepository.ReplacePermissionsAsync(role.Id, permissionIds);
            await _roleRepository.SaveChangesAsync();

            return await GetRoleByIdAsync(role.Id);
        }

        public async Task<RoleResponse> UpdateRoleAsync(Guid id, RoleRequest request)
        {
            await ValidationRunner.EnsureValidAsync(_validator, request);

            var role = await _roleRepository.GetWithAllRelationsAsync(id)
                       ?? throw new NotFoundException($"Không tìm thấy vai trò với Id '{id}'.");

            var code = request.Code.Trim().ToUpperInvariant();
            var isAdmin = role.Code == AdminRoleCode;

            if (isAdmin && code != AdminRoleCode)
            {
                throw new ConflictException("Không thể đổi mã của vai trò ADMIN.");
            }

            if (!isAdmin && code != role.Code)
            {
                var conflict = await _roleRepository.GetByCodeAsync(code);
                if (conflict != null && conflict.Id != id)
                {
                    throw new ConflictException($"Mã vai trò '{code}' đã tồn tại.");
                }
            }

            role.Code = code;
            role.Name = request.Name.Trim();
            role.Description = request.Description?.Trim() ?? string.Empty;

            var permissionIds = await EnsurePermissionsExistAsync(request.PermissionIds);

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();

            await _roleRepository.ReplacePermissionsAsync(id, permissionIds);
            await _roleRepository.SaveChangesAsync();

            return await GetRoleByIdAsync(id);
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            var role = await _roleRepository.GetWithAllRelationsAsync(id)
                       ?? throw new NotFoundException($"Không tìm thấy vai trò với Id '{id}'.");

            if (role.Code == AdminRoleCode)
            {
                throw new ConflictException("Không thể xóa vai trò ADMIN.");
            }

            var removed = await _roleRepository.HardDeleteAsync(id);
            if (!removed)
            {
                throw new NotFoundException($"Không tìm thấy vai trò với Id '{id}'.");
            }

            await _roleRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Loại bỏ Id trùng/rỗng và bảo đảm mọi quyền hạn đều tồn tại trong DB,
        /// tránh tạo bản ghi join trỏ tới permission không có thật.
        /// </summary>
        private async Task<IReadOnlyCollection<Guid>> EnsurePermissionsExistAsync(IEnumerable<Guid>? permissionIds)
        {
            var requested = (permissionIds ?? Array.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (requested.Count == 0)
            {
                return Array.Empty<Guid>();
            }

            var all = await _roleRepository.GetAllPermissionsAsync();
            var found = all.Select(p => p.Id).ToHashSet();

            var missing = requested.Where(id => !found.Contains(id)).ToList();
            if (missing.Count > 0)
            {
                throw new BadRequestException(
                    "Danh sách quyền hạn chứa quyền không tồn tại.",
                    missing.Select(id => $"Quyền hạn không tồn tại: {id}").ToList());
            }

            return requested;
        }
    }
}