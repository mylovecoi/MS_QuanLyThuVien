namespace IdentityService.Business.Dtos.Responses
{
    /// <summary>
    /// Nhóm quyền hạn theo Module/Service, dùng để render ma trận phân quyền trên giao diện.
    /// </summary>
    public class PermissionGroupResponse
    {
        /// <summary>Code của module, ví dụ: "Catalog", "Circulation".</summary>
        public string Module { get; set; } = string.Empty;

        /// <summary>Tên hiển thị tiếng Việt của module.</summary>
        public string ModuleLabel { get; set; } = string.Empty;

        /// <summary>Thứ tự hiển thị trên ma trận UI (nhỏ hơn = hiển thị trước).</summary>
        public int SortOrder { get; set; }

        /// <summary>Danh sách quyền thuộc module này, đã được sắp xếp theo thứ tự action chuẩn.</summary>
        public List<PermissionResponse> Permissions { get; set; } = [];
    }
}
