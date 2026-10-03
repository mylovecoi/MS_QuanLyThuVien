using IdentityService.Business.Dtos.Responses;
using IdentityService.DataAccess.Entities;

namespace IdentityService.Business.Mappers
{
    public static class PermissionMapper
    {
        public static PermissionResponse ToResponse(this PermissionEntity entity)
        {
            return new PermissionResponse
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Module = entity.Module,
                Description = entity.Description
            };
        }
    }
}
