export const API_ENDPOINTS = {
  IDENTITY: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    ME: '/auth/me',
  },
  PROFILE: {
    USER_PROFILE: '/userprofile',
    BY_USER: (userId) => `/userprofile/by-user/${userId}`,
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
