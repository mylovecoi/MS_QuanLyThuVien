using IdentityService.Business.Dtos.Responses;
using IdentityService.DataAccess.Entities;

namespace IdentityService.Business.Mappers
{
    public static class RoleMapper
    {
        public static RoleResponse ToResponse(this RoleEntity entity)
        {
            var permissions = entity.RolePermissions?
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission.ToResponse())
                .DistinctBy(p => p.Id)
                .ToList() ?? new List<PermissionResponse>();

            return new RoleResponse
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                Permissions = permissions
            };
        }
    }
}
