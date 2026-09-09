import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { Alert } from '@/components/feedback/Alert';

export const RegisterPage = () => {
  const [formData, setFormData] = useState({ username: '', email: '', password: '', fullName: '' });
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await authApi.register(formData);
      navigate('/login');
    } catch (err) {
      setError(err.message || 'Đăng ký thất bại. Vui lòng kiểm tra lại thông tin.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div style={{ minHeight: 'calc(100vh - 64px)', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '24px' }}>
      <form onSubmit={handleSubmit} className="card" style={{ width: '100%', maxWidth: '420px' }}>
        <h2 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: '20px', textAlign: 'center' }}>
          Đăng ký tài khoản
        </h2>

        <Alert message={error} type="error" />

        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
          <Input label="Tên đăng nhập" value={formData.username} onChange={(e) => setFormData({ ...formData, username: e.target.value })} required />
          <Input label="Email" type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} required />
          <Input label="Họ và tên" value={formData.fullName} onChange={(e) => setFormData({ ...formData, fullName: e.target.value })} required />
          <Input label="Mật khẩu" type="password" value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} required />

          <Button type="submit" isLoading={isLoading} style={{ marginTop: '8px' }}>Tạo tài khoản</Button>
        </div>

        <p style={{ marginTop: '20px', textAlign: 'center', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
          Đã có tài khoản? <Link to="/login">Đăng nhập</Link>
        </p>
      </form>
    </div>
  );
};
