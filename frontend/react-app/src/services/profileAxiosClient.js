import axios from 'axios';

const profileClient = axios.create({
  baseURL: import.meta.env.VITE_PROFILE_SERVICE_URL || 'http://localhost:5006/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000, // Tăng timeout lên 30s để chờ backend khởi động
});

profileClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

profileClient.interceptors.response.use(
  (response) => {
    return response.data?.data !== undefined ? response.data.data : response.data;
  },
  (error) => {
    if (error.code === 'ECONNABORTED' || error.message?.includes('timeout')) {
      return Promise.reject({
        message: 'Kết nối đến ProfileService (Port 5006) quá thời gian chờ (Timeout). Vui lòng kiểm tra backend đã bật chưa.',
      });
    }
    return Promise.reject(error.response?.data || error);
  }
);

export default profileClient;
