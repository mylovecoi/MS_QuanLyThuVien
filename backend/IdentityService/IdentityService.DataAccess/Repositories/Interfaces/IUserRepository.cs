using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetWithRolesAndPermissionsByIdAsync(Guid userId);
        Task<UserEntity?> GetWithRolesAndPermissionsByUsernameAsync(string username);

        /// <summary>
        /// Nạp user kèm danh sách vai trò (không cần permission) để hiển thị/sửa.
        /// </summary>
        Task<UserEntity?> GetWithRolesByIdAsync(Guid userId);

        /// <summary>
        /// Phân trang ở tầng DB, nạp UserRoles cho đúng 1 trang.
        /// <paramref name="search"/> lọc theo username/email/họ tên (không phân biệt hoa thường).
        /// </summary>
        Task<PagedResult> GetPagedWithRolesAsync(int page, int pageSize, string? search);

        /// <summary>
        /// Thay toàn bộ vai trò của user. Chỉ thêm/xóa phần chênh lệch để tránh
        /// lỗi tracking của EF khi các bản ghi cũ vẫn ở trạng thái Deleted.
        /// </summary>
        Task ReplaceRolesAsync(Guid userId, IReadOnlyCollection<Guid> roleIds);
    }
}
