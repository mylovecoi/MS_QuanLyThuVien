import axiosClient from '@/services/axiosClient';
import { API_ENDPOINTS } from '@/utils/constants';

export const userApi = {
  getUsers: (page = 1, pageSize = 20, search = '') =>
    axiosClient.get(API_ENDPOINTS.USER.LIST, {
      params: { page, pageSize, search: search || undefined },
    }),

  getUser: (id) => axiosClient.get(API_ENDPOINTS.USER.BY_ID(id)),

  createUser: (data) => axiosClient.post(API_ENDPOINTS.USER.LIST, data),

  updateUser: (id, data) => axiosClient.put(API_ENDPOINTS.USER.BY_ID(id), data),

  updateStatus: (id, status) =>
    axiosClient.put(API_ENDPOINTS.USER.STATUS(id), { status }),

  resetPassword: (id, newPassword) =>
    axiosClient.put(API_ENDPOINTS.USER.PASSWORD(id), { newPassword }),

  deleteUser: (id) => axiosClient.delete(API_ENDPOINTS.USER.BY_ID(id)),
};
