import React, { createContext, useState, useEffect } from 'react';
import axiosClient from '@/services/axiosClient';
import { authApi } from '@/features/auth/api/authApi';
import { tokenStorage } from '@/utils/tokenStorage';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = async () => {
      const token = tokenStorage.getToken();
      if (token) {
        try {
          // Lấy thông tin user hiện tại nếu có token
          const userData = await axiosClient.get('/auth/me');
          setUser(userData);
        } catch {
          tokenStorage.clear();
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = (authData, remember = true) => {
    tokenStorage.setTokens(
      { accessToken: authData.accessToken, refreshToken: authData.refreshToken },
      remember
    );
    setUser(authData.user);
  };

  const logout = async () => {
    const refreshToken = tokenStorage.getRefreshToken();
    // Gọi API để thu hồi token phía server (vô hiệu hóa access token hiện tại
    // và refresh token). Lỗi không chặn việc đăng xuất phía client.
    try {
      await authApi.logout({ refreshToken });
    } catch {
      /* bỏ qua */
    }
    tokenStorage.clear();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, setUser, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};
