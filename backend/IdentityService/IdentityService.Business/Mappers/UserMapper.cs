using IdentityService.Business.Dtos.Responses;
using IdentityService.DataAccess.Entities;

namespace IdentityService.Business.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(this UserEntity entity)
        {
            var roles = entity.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role.Code)
                .Distinct()
                .ToList() ?? new System.Collections.Generic.List<string>();

            var permissions = entity.UserRoles?
                .Where(ur => ur.Role != null && ur.Role.RolePermissions != null)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToList() ?? new System.Collections.Generic.List<string>();

            return new UserResponse
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                FullName = entity.FullName,
                Role = roles.FirstOrDefault() ?? entity.Role,
                Roles = roles,
                Permissions = permissions
            };
        }
    }
}
