import axiosClient from '@/services/axiosClient';
import { API_ENDPOINTS } from '@/utils/constants';

export const authApi = {
  login: (data) => axiosClient.post(API_ENDPOINTS.IDENTITY.LOGIN, data),
  register: (data) => axiosClient.post(API_ENDPOINTS.IDENTITY.REGISTER, data),
  getMe: () => axiosClient.get(API_ENDPOINTS.IDENTITY.ME),
  logout: (data) => axiosClient.post('/auth/logout', data),
};
