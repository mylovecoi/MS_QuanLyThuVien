import React, { createContext, useState, useEffect } from 'react';
import axiosClient from '@/services/axiosClient';
import { authApi } from '@/features/auth/api/authApi';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          // Lấy thông tin user hiện tại nếu có token
          const userData = await axiosClient.get('/auth/me');
          setUser(userData);
        } catch (err) {
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = (authData) => {
    localStorage.setItem('token', authData.accessToken);
    localStorage.setItem('refreshToken', authData.refreshToken);
    setUser(authData.user);
  };

  const logout = async () => {
    const refreshToken = localStorage.getItem('refreshToken');
    // Gọi API để thu hồi token phía server (vô hiệu hóa access token hiện tại
    // và refresh token). Lỗi không chặn việc đăng xuất phía client.
    try {
      await authApi.logout({ refreshToken });
    } catch {
      /* bỏ qua */
    }
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, setUser, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};
