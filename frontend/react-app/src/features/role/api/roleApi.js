import axiosClient from '@/services/axiosClient';
import { API_ENDPOINTS } from '@/utils/constants';

// Danh sách role trả về PagedResult. Mặc định lấy trang đầu với pageSize lớn
// để bảng quản lý hiển thị được toàn bộ vai trò.
const LARGE_PAGE_SIZE = 200;

export const roleApi = {
  getRoles: (page = 1, pageSize = LARGE_PAGE_SIZE) =>
    axiosClient.get(API_ENDPOINTS.ROLE.LIST, { params: { page, pageSize } }),

  /** Flat list — giữ nguyên để backward-compatible */
  getPermissions: () => axiosClient.get(API_ENDPOINTS.ROLE.PERMISSIONS),

  /**
   * Permissions được nhóm theo Module/Service.
   * Trả về mảng PermissionGroupResponse, mỗi group gồm:
   *   { module, moduleLabel, sortOrder, permissions: PermissionResponse[] }
   */
  getPermissionsGrouped: () => axiosClient.get(API_ENDPOINTS.ROLE.PERMISSIONS_GROUPED),

  createRole: (data) => axiosClient.post(API_ENDPOINTS.ROLE.LIST, data),

  updateRole: (id, data) => axiosClient.put(API_ENDPOINTS.ROLE.BY_ID(id), data),

  deleteRole: (id) => axiosClient.delete(API_ENDPOINTS.ROLE.BY_ID(id)),
};