import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { useTheme } from '@/hooks/useTheme';

export const Header = () => {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header style={{
      height: '64px',
      borderBottom: '1px solid var(--border-color)',
      backgroundColor: 'var(--bg-card)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      padding: '0 24px'
    }}>
      <Link to="/" style={{ fontSize: '1.25rem', fontWeight: 700, color: 'var(--text-primary)' }}>
        ⚡ Identity<span style={{ color: 'var(--accent-color)' }}>Platform</span>
      </Link>
      <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
        {/* Nút chuyển đổi Light / Dark Theme */}
        <button className="theme-toggle-btn" onClick={toggleTheme} title="Chuyển chế độ Giao diện">
          {theme === 'light' ? '🌙 Dark' : '☀️ Light'}
        </button>

        {user ? (
          <>
            <span style={{ fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
              Xin chào, <strong>{user.fullName || user.username}</strong>
            </span>
            <button className="btn btn-secondary" style={{ padding: '6px 12px', fontSize: '0.85rem' }} onClick={handleLogout}>
              Đăng xuất
            </button>
          </>
        ) : (
          <Link to="/login" className="btn btn-primary" style={{ padding: '6px 14px', fontSize: '0.85rem' }}>
            Đăng nhập
          </Link>
        )}
      </div>
    </header>
  );
};
