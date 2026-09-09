import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { useAuth } from '@/hooks/useAuth';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Alert } from '@/components/feedback/Alert';

export const LoginForm = () => {
  const [formData, setFormData] = useState({ username: '', password: '' });
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      const response = await authApi.login(formData);
      login(response);
      navigate('/profile');
    } catch (err) {
      setError(err.message || 'Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="card" style={{ width: '100%', maxWidth: '400px' }}>
      <h2 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: '20px', textAlign: 'center' }}>
        Đăng nhập
      </h2>

      <Alert message={error} type="error" />

      <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
        <Input
          label="Tên đăng nhập"
          placeholder="Nhập username"
          value={formData.username}
          onChange={(e) => setFormData({ ...formData, username: e.target.value })}
          required
        />

        <Input
          label="Mật khẩu"
          type="password"
          placeholder="••••••••"
          value={formData.password}
          onChange={(e) => setFormData({ ...formData, password: e.target.value })}
          required
        />

        <Button type="submit" isLoading={isLoading} style={{ marginTop: '8px' }}>
          Đăng nhập
        </Button>
      </div>

      <p style={{ marginTop: '20px', textAlign: 'center', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
        Chưa có tài khoản? <Link to="/register">Đăng ký ngay</Link>
      </p>
    </form>
  );
};
