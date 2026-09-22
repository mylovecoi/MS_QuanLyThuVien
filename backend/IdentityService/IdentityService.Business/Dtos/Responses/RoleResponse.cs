namespace IdentityService.Business.Dtos.Responses
{
    public class RoleResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PermissionResponse> Permissions { get; set; } = new List<PermissionResponse>();
    }
}
