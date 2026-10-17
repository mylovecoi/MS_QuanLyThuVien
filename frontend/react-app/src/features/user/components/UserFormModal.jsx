import React, { useEffect, useState } from 'react';
import { Modal } from '@/components/ui/Modal';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Button } from '@/components/ui/Button';
import { ACCOUNT_STATUS_OPTIONS, GENDER_OPTIONS } from '@/utils/constants';

const EMPTY_FORM = {
  username: '',
  email: '',
  fullName: '',
  password: '',
  gender: 0,
  status: 1,
  roleIds: [],
};

export const UserFormModal = ({
  isOpen,
  onClose,
  editing,
  roles = [],
  saving = false,
  errors = {},
  onSubmit,
}) => {
  const [form, setForm] = useState(EMPTY_FORM);
  const isEdit = Boolean(editing);

  useEffect(() => {
    if (!isOpen) return;
    if (editing) {
      setForm({
        username: editing.username || '',
        email: editing.email || '',
        fullName: editing.fullName || '',
        password: '',
        gender: editing.gender ?? 0,
        status: editing.status ?? 1,
        roleIds: (editing.roles || []).map((r) => r.id),
      });
    } else {
      setForm(EMPTY_FORM);
    }
  }, [isOpen, editing]);

  const toggleRole = (roleId) => {
    setForm((prev) => {
      const set = new Set(prev.roleIds);
      if (set.has(roleId)) set.delete(roleId);
      else set.add(roleId);
      return { ...prev, roleIds: [...set] };
    });
  };

  const handleSubmit = (e) => {
    if (e) e.preventDefault();
    const payload = {
      email: form.email.trim(),
      fullName: form.fullName.trim(),
      gender: Number(form.gender),
      roleIds: form.roleIds,
    };
    if (!isEdit) {
      payload.username = form.username.trim();
      payload.password = form.password;
      payload.status = Number(form.status);
    }
    onSubmit(payload);
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEdit ? `Sửa tài khoản: ${editing?.username}` : 'Tạo tài khoản'}
      maxWidth="560px"
      footer={
        <>
          <Button variant="ghost" onClick={onClose} disabled={saving}>
            Hủy
          </Button>
          <Button onClick={handleSubmit} isLoading={saving}>
            {isEdit ? 'Lưu thay đổi' : 'Tạo tài khoản'}
          </Button>
        </>
      }
    >
      <form onSubmit={handleSubmit}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          <Input
            label="Tên đăng nhập"
            required
            value={form.username}
            onChange={(e) => setForm((p) => ({ ...p, username: e.target.value }))}
            placeholder="VD: nguyenvana"
            disabled={isEdit}
            error={errors.username}
          />

          <Input
            label="Email"
            type="email"
            required
            value={form.email}
            onChange={(e) => setForm((p) => ({ ...p, email: e.target.value }))}
            placeholder="VD: email@example.com"
            error={errors.email}
          />

          <Input
            label="Họ và tên"
            required
            value={form.fullName}
            onChange={(e) => setForm((p) => ({ ...p, fullName: e.target.value }))}
            placeholder="VD: Nguyễn Văn A"
            error={errors.fullName}
          />

          {!isEdit && (
            <Input
              label="Mật khẩu"
              type="password"
              required
              value={form.password}
              onChange={(e) => setForm((p) => ({ ...p, password: e.target.value }))}
              placeholder="Tối thiểu 6 ký tự, ít nhất 1 chữ và 1 số"
              error={errors.password}
            />
          )}

          <div style={{ display: 'flex', gap: 16 }}>
            <Select
              label="Giới tính"
              value={form.gender}
              onChange={(e) => setForm((p) => ({ ...p, gender: Number(e.target.value) }))}
              options={GENDER_OPTIONS}
            />
            {!isEdit && (
              <Select
                label="Trạng thái"
                value={form.status}
                onChange={(e) => setForm((p) => ({ ...p, status: Number(e.target.value) }))}
                options={ACCOUNT_STATUS_OPTIONS}
              />
            )}
          </div>

          <div className="form-group">
            <label className="form-label">Vai trò</label>
            {roles.length === 0 ? (
              <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', margin: 0 }}>
                Chưa tải được danh sách vai trò.
              </p>
            ) : (
              <div
                style={{
                  maxHeight: 180,
                  overflowY: 'auto',
                  border: '1px solid var(--border-color)',
                  borderRadius: 'var(--radius-md)',
                  padding: '8px 12px',
                  display: 'flex',
                  flexDirection: 'column',
                  gap: 8,
                }}
              >
                {roles.map((role) => (
                  <label
                    key={role.id}
                    style={{ display: 'flex', alignItems: 'center', gap: 8, cursor: 'pointer', fontSize: '0.9rem' }}
                  >
                    <input
                      type="checkbox"
                      checked={form.roleIds.includes(role.id)}
                      onChange={() => toggleRole(role.id)}
                      style={{ width: 16, height: 16, accentColor: 'var(--accent-color)', cursor: 'pointer' }}
                    />
                    <span style={{ fontWeight: 600 }}>{role.name}</span>
                    <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', letterSpacing: '0.04em' }}>
                      {role.code}
                    </span>
                  </label>
                ))}
              </div>
            )}
            {errors.roleIds && <span className="form-error">{errors.roleIds}</span>}
          </div>
        </div>
        <button type="submit" style={{ display: 'none' }} aria-hidden="true" />
      </form>
    </Modal>
  );
};
