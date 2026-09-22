namespace IdentityService.DataAccess.Entities
{
    public class PermissionEntity : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
    }
}
