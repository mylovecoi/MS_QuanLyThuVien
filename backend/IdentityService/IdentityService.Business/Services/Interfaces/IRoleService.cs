using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using IdentityService.DataAccess.Repositories.Interfaces;
using Shared.Contracts.Responses;

namespace IdentityService.Business.Services.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResult<RoleResponse>> GetRolesAsync(int page, int pageSize);
        Task<RoleResponse> GetRoleByIdAsync(Guid id);
        Task<IReadOnlyList<PermissionResponse>> GetAllPermissionsAsync();
        Task<IReadOnlyList<PermissionGroupResponse>> GetPermissionsGroupedAsync();
        Task<RoleResponse> CreateRoleAsync(RoleRequest request);
        Task<RoleResponse> UpdateRoleAsync(Guid id, RoleRequest request);
        Task DeleteRoleAsync(Guid id);
    }
}