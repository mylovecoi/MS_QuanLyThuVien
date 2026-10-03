using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<RoleEntity>
    {
        Task<RoleEntity?> GetByCodeAsync(string code);
        Task<RoleEntity?> GetWithPermissionsAsync(Guid roleId);
        Task<IEnumerable<RoleEntity>> GetAllWithPermissionsAsync();

        /// <summary>
        /// Nạp role kèm cả RolePermissions và UserRoles để cập nhật/xóa có kiểm soát.
        /// </summary>
        Task<RoleEntity?> GetWithAllRelationsAsync(Guid roleId);

        /// <summary>
        /// Phân trang ở tầng DB, đồng thời nạp RolePermissions cho đúng 1 trang.
        /// </summary>
        Task<PagedResult> GetPagedWithPermissionsAsync(int page, int pageSize);

        /// <summary>
        /// Danh sách toàn bộ quyền hạn trong hệ thống, dùng cho danh sách lựa chọn khi phân quyền.
        /// </summary>
        Task<IReadOnlyList<PermissionEntity>> GetAllPermissionsAsync();

        /// <summary>
        /// Số người dùng đang được gán vai trò này.
        /// </summary>
        Task<int> CountUserRolesAsync(Guid roleId);

        /// <summary>
        /// Thay toàn bộ quyền hạn của role. Xóa cứng các bản ghi RolePermission cũ
        /// rồi tạo mới, tránh dồn dữ liệu rác mỗi lần cập nhật.
        /// </summary>
        Task ReplacePermissionsAsync(Guid roleId, IReadOnlyCollection<Guid> permissionIds);

        /// <summary>
        /// Xóa cứng role cùng các bản ghi liên kết (RolePermission, UserRole).
        /// Dùng cho xóa thật; còn xóa mềm qua IRepository.Delete chỉ ẩn role đi.
        /// </summary>
        Task<bool> HardDeleteAsync(Guid roleId);
    }
}