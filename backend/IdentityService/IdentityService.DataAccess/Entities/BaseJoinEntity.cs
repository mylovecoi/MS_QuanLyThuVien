namespace IdentityService.DataAccess.Entities
{
    /// <summary>
    /// Base class cho các join/pivot table entity (composite key, không có Id riêng).
    /// </summary>
    public abstract class BaseJoinEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
