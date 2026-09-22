using IdentityService.DataAccess.Enums;

namespace IdentityService.DataAccess.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Unknown;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public string Role { get; set; } = "User";

        public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
        public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = [];
    }
}
