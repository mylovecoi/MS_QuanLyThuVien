using System.Linq;
using IdentityService.Business.Dtos.Responses;
using IdentityService.DataAccess.Entities;

namespace IdentityService.Business.Mappers
{
    public static class UserMapper
    {
        /// <summary>
        /// Map cho trang quản lý tài khoản: kèm danh sách vai trò, trạng thái, thời gian.
        /// Không đưa mật khẩu hay permission ra ngoài.
        /// </summary>
        public static UserDetailResponse ToDetailResponse(this UserEntity entity)
        {
            var roles = entity.UserRoles?
                .Where(ur => ur.Role != null)
                .Select(ur => new UserRoleResponse
                {
                    Id = ur.Role.Id,
                    Code = ur.Role.Code,
                    Name = ur.Role.Name
                })
                .DistinctBy(r => r.Id)
                .OrderBy(r => r.Code)
                .ToList() ?? new System.Collections.Generic.List<UserRoleResponse>();

            return new UserDetailResponse
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                FullName = entity.FullName,
                Gender = (int)entity.Gender,
                Status = (int)entity.Status,
                Roles = roles,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

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
