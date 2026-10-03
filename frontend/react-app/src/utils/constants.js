export const API_ENDPOINTS = {
  IDENTITY: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    ME: '/auth/me',
    // Phải khớp route [HttpPost("refresh-token")] ở AuthController
    REFRESH_TOKEN: '/auth/refresh-token',
  },
  ROLE: {
    LIST: '/role',
    PERMISSIONS: '/role/permissions',
    BY_ID: (id) => `/role/${id}`,
  },
  PROFILE: {
    USER_PROFILE: '/profile',
    BY_USER: (userId) => `/profile/by-user/${userId}`,
    PHONG_BAN: '/phongban',
    CHUC_VU: '/chucvu',
  },
};

export const ROLES = {
  ADMIN: 'Admin',
  USER: 'User',
};

export const GENDER_OPTIONS = [
  { value: 0, label: 'Chưa xác định' },
  { value: 1, label: 'Nam' },
  { value: 2, label: 'Nữ' },
  { value: 3, label: 'Khác' },
];
