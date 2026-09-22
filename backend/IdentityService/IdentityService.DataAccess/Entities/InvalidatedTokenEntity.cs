namespace IdentityService.DataAccess.Entities
{
    public class InvalidatedTokenEntity : BaseEntity
    {
        public string Jti { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
