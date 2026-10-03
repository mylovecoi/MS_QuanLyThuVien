import profileClient from '@/services/profileAxiosClient';
import { API_ENDPOINTS } from '@/utils/constants';

// Các endpoint danh sách trả về PagedResult nên cần truyền page/pageSize.
// Mặc định lấy trang đầu với pageSize lớn để các dropdown chọn không bị cắt.
const LARGE_PAGE_SIZE = 200;

export const profileApi = {
  getProfileByUserId: (userId) => profileClient.get(API_ENDPOINTS.PROFILE.BY_USER(userId)),

  createProfile: (profileData) => profileClient.post(API_ENDPOINTS.PROFILE.USER_PROFILE, profileData),

  updateProfileByUserId: (userId, profileData) => profileClient.put(API_ENDPOINTS.PROFILE.BY_USER(userId), profileData),

  deleteProfile: (id) => profileClient.delete(`${API_ENDPOINTS.PROFILE.USER_PROFILE}/${id}`),

  getPhongBans: (page = 1, pageSize = LARGE_PAGE_SIZE) =>
    profileClient.get(API_ENDPOINTS.PROFILE.PHONG_BAN, { params: { page, pageSize } }),

  getChucVus: (page = 1, pageSize = LARGE_PAGE_SIZE) =>
    profileClient.get(API_ENDPOINTS.PROFILE.CHUC_VU, { params: { page, pageSize } }),
};
