import axios from 'axios';

const identityClient = axios.create({
  baseURL: import.meta.env.VITE_IDENTITY_SERVICE_URL || 'http://localhost:5005/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000, // Tăng timeout lên 30s để chờ backend khởi động
});

identityClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

identityClient.interceptors.response.use(
  (response) => {
    return response.data?.data !== undefined ? response.data.data : response.data;
  },
  (error) => {
    if (error.code === 'ECONNABORTED' || error.message?.includes('timeout')) {
      return Promise.reject({
        message: 'Kết nối đến IdentityService (Port 5005) quá thời gian chờ (Timeout). Vui lòng kiểm tra backend đã bật chưa.',
      });
    }
    return Promise.reject(error.response?.data || error);
  }
);

export default identityClient;
