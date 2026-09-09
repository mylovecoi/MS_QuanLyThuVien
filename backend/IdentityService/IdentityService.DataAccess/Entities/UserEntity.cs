using System;
using IdentityService.DataAccess.Enums;

namespace IdentityService.DataAccess.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class UserEntity : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Unknown;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public string Role { get; set; } = "User";
    }
}
