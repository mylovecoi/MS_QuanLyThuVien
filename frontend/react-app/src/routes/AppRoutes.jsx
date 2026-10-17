import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from '@/features/auth/pages/LoginPage';
import { RegisterPage } from '@/features/auth/pages/RegisterPage';
import { ProfilePage } from '@/features/profile/pages/ProfilePage';
import { RolePage } from '@/features/role/pages/RolePage';
import { UserPage } from '@/features/user/pages/UserPage';
import { FileListPage } from '@/features/file/pages/FileListPage';
import { AppLayout } from '@/components/layout/AppLayout';
import { ProtectedRoute } from './ProtectedRoute';

export const AppRoutes = () => {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Protected Routes — có Sidebar */}
      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/roles" element={<RolePage />} />
          <Route path="/users" element={<UserPage />} />
          <Route path="/files" element={<FileListPage />} />
          <Route path="/" element={<Navigate to="/profile" replace />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
};
