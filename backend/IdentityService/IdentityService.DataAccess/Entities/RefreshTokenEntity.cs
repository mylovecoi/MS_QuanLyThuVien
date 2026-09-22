namespace IdentityService.DataAccess.Entities
{
    public class RefreshTokenEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
