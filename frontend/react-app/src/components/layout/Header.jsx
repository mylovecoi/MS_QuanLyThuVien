import React from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { useTheme } from '@/hooks/useTheme';

export const Header = () => {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const canReadRoles = user?.permissions?.includes('roles.read');

  const navLinkStyle = (isActive) => ({
    fontSize: '0.9rem',
    fontWeight: 600,
    color: isActive ? 'var(--accent-color)' : 'var(--text-secondary)',
    textDecoration: 'none',
    padding: '4px 2px',
    borderBottom: isActive ? '2px solid var(--accent-color)' : '2px solid transparent',
  });

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
        {/* Điều hướng chính: chỉ hiện mục mà user đang có quyền xem */}
        {user && (
          <nav style={{ display: 'flex', alignItems: 'center', gap: '16px', marginRight: '8px' }}>
            <Link
              to="/profile"
              style={navLinkStyle(location.pathname.startsWith('/profile'))}
            >
              Hồ sơ
            </Link>
            {canReadRoles && (
              <Link
                to="/roles"
                style={navLinkStyle(location.pathname.startsWith('/roles'))}
              >
                Vai trò
              </Link>
            )}
          </nav>
        )}

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
