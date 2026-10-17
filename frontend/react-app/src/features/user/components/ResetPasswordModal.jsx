import React, { useState } from 'react';
import { Modal } from '@/components/ui/Modal';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';

export const ResetPasswordModal = ({
  isOpen,
  onClose,
  username,
  saving = false,
  error = '',
  onSubmit,
}) => {
  const [newPassword, setNewPassword] = useState('');
  const [confirm, setConfirm] = useState('');

  const handleSubmit = (e) => {
    if (e) e.preventDefault();
    onSubmit(newPassword);
    setNewPassword('');
    setConfirm('');
  };

  const isValid = newPassword.length >= 6 && /[A-Za-z]/.test(newPassword) && /\d/.test(newPassword) && newPassword === confirm;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={`Đặt lại mật khẩu: ${username || ''}`}
      maxWidth="480px"
      footer={
        <>
          <Button variant="ghost" onClick={onClose} disabled={saving}>
            Hủy
          </Button>
          <Button onClick={handleSubmit} isLoading={saving} disabled={!isValid}>
            Lưu
          </Button>
        </>
      }
    >
      <form onSubmit={handleSubmit}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          <Input
            label="Mật khẩu mới"
            type="password"
            required
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            placeholder="Tối thiểu 6 ký tự, ít nhất 1 chữ và 1 số"
          />
          <Input
            label="Xác nhận mật khẩu"
            type="password"
            required
            value={confirm}
            onChange={(e) => setConfirm(e.target.value)}
            placeholder="Nhập lại mật khẩu mới"
            error={confirm && newPassword !== confirm ? 'Xác nhận mật khẩu không khớp' : ''}
          />
          {error && <span className="form-error">{error}</span>}
        </div>
        <button type="submit" style={{ display: 'none' }} aria-hidden="true" />
      </form>
    </Modal>
  );
};
