import profileClient from '@/services/profileAxiosClient';
import { API_ENDPOINTS } from '@/utils/constants';

export const profileApi = {
  getProfileByUserId: (userId) => profileClient.get(API_ENDPOINTS.PROFILE.BY_USER(userId)),
  
  createProfile: (profileData) => profileClient.post(API_ENDPOINTS.PROFILE.USER_PROFILE, profileData),
  
  updateProfileByUserId: (userId, profileData) => profileClient.put(API_ENDPOINTS.PROFILE.BY_USER(userId), profileData),
  
  getPhongBans: () => profileClient.get(API_ENDPOINTS.PROFILE.PHONG_BAN),
  
  getChucVus: () => profileClient.get(API_ENDPOINTS.PROFILE.CHUC_VU),
};
