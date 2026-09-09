import axiosClient from '@/services/axiosClient';

export const authApi = {
  login: (data) => axiosClient.post('/auth/login', data),
  register: (data) => axiosClient.post('/auth/register', data),
  getMe: () => axiosClient.get('/auth/me'),
};
