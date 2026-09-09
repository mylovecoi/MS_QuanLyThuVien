using IdentityService.Business.Dtos.Responses;
using IdentityService.DataAccess.Entities;

namespace IdentityService.Business.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToResponse(this UserEntity entity)
        {
            return new UserResponse
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                FullName = entity.FullName,
                Role = entity.Role
            };
        }
    }
}
