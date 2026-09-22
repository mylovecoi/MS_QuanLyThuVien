namespace IdentityService.DataAccess.Entities
{
    public class RolePermissionEntity : BaseJoinEntity
    {
        public Guid RoleId { get; set; }
        public RoleEntity Role { get; set; } = null!;

        public Guid PermissionId { get; set; }
        public PermissionEntity Permission { get; set; } = null!;
    }
}
