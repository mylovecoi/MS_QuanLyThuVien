import axios from 'axios';
import { normalizeApiError } from '@/utils/apiError';

// Tất cả request đi qua API Gateway (Port 5000)
// Gateway sẽ forward tới đúng microservice tương ứng
const GATEWAY_URL = import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5000/api';

const apiClient = axios.create({
  baseURL: GATEWAY_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
  (response) => {
    // Download binary: giữ nguyên Blob, không unwrap envelope JSON.
    if (response.config?.responseType === 'blob') {
      return response.data;
    }

    // 204 No Content: không có body để unwrap
    if (response.status === 204 || !response.data) {
      return null;
    }

    // Backend trả envelope { success, message, data, errors, traceId }.
    // Trả về đúng phần data cho phần lớn call site.
    if (response.data.success !== undefined && 'data' in response.data) {
      return response.data.data;
    }

    return response.data;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.code === 'ECONNABORTED' || error.message?.includes('timeout')) {
      return Promise.reject({
        message: 'Kết nối đến API Gateway (Port 5000) quá thời gian chờ. Vui lòng kiểm tra backend đã khởi động chưa.',
      });
    }

    // Auto refresh token khi 401
    if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const res = await axios.post(`${GATEWAY_URL}/auth/refresh-token`, { refreshToken });
          const newAccessToken = res.data?.data?.accessToken;
          if (!newAccessToken) throw new Error('Refresh token không hợp lệ.');

          localStorage.setItem('token', newAccessToken);
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
          return apiClient(originalRequest);
        }
        throw new Error('Không có refresh token.');
      } catch {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
        return Promise.reject(error);
      }
    }

    // Mọi nhánh reject đều phải ra cùng một shape { message, errors, status,
    // traceId }. Nhánh này trước đây reject thẳng error.response?.data hoặc error
    // của axios, nên khi body không phải envelope (lỗi gateway, HTML error page,
    // body rỗng) UI sẽ hiện "Request failed with status code 400" thay vì
    // thông báo của backend.
    return Promise.reject(normalizeApiError(error, 'Đã xảy ra lỗi.'));
  }
);

export default apiClient;
