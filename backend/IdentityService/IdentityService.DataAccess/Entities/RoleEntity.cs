namespace IdentityService.DataAccess.Entities
{
    public class RoleEntity : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
    }
}
