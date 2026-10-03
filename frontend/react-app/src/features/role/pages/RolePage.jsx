import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { roleApi } from '@/features/role/api/roleApi';
import { useAuth } from '@/hooks/useAuth';
import { Alert } from '@/components/feedback/Alert';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Modal } from '@/components/ui/Modal';

// ADMIN là vai trò hệ thống: sửa được nhưng không xoá được.
const PROTECTED_ROLE_CODE = 'ADMIN';

const EMPTY_FORM = { code: '', name: '', description: '', permissionIds: [] };

const tableStyle = { width: '100%', borderCollapse: 'collapse' };
const thStyle = {
  textAlign: 'left',
  padding: '10px 12px',
  borderBottom: '1px solid var(--border-color)',
  fontSize: '0.85rem',
  color: 'var(--text-secondary)',
  fontWeight: 600,
};
const tdStyle = {
  padding: '10px 12px',
  borderBottom: '1px solid var(--border-color)',
  fontSize: '0.9rem',
  color: 'var(--text-primary)',
  verticalAlign: 'top',
};

export const RolePage = () => {
  const { user } = useAuth();

  const canRead = user?.permissions?.includes('roles.read');
  const canWrite = user?.permissions?.includes('roles.write');

  const [roles, setRoles] = useState([]);
  const [allPermissions, setAllPermissions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form, setForm] = useState(EMPTY_FORM);
  const [saving, setSaving] = useState(false);
  const [formErrors, setFormErrors] = useState({});

  const loadRoles = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const [rolePage, permissions] = await Promise.all([
        roleApi.getRoles(),
        roleApi.getPermissions(),
      ]);
      setRoles(rolePage?.items ?? []);
      setAllPermissions(permissions ?? []);
    } catch (err) {
      setError(err.message || 'Không tải được danh sách vai trò.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (canRead) {
      loadRoles();
    } else {
      setLoading(false);
    }
  }, [canRead, loadRoles]);

  // Gom quyền hạn theo module để form phân quyền dễ đọc.
  const permissionsByModule = useMemo(() => {
    const groups = new Map();
    allPermissions.forEach((p) => {
      const module = p.module || 'Khác';
      if (!groups.has(module)) groups.set(module, []);
      groups.get(module).push(p);
    });
    return [...groups.entries()];
  }, [allPermissions]);

  const openCreate = () => {
    setEditing(null);
    setForm(EMPTY_FORM);
    setFormErrors({});
    setError('');
    setSuccess('');
    setFormOpen(true);
  };

  const openEdit = (role) => {
    setEditing(role);
    setForm({
      code: role.code,
      name: role.name,
      description: role.description || '',
      permissionIds: (role.permissions || []).map((p) => p.id),
    });
    setFormErrors({});
    setError('');
    setSuccess('');
    setFormOpen(true);
  };

  const togglePermission = (permissionId) => {
    setForm((prev) => {
      const has = prev.permissionIds.includes(permissionId);
      return {
        ...prev,
        permissionIds: has
          ? prev.permissionIds.filter((id) => id !== permissionId)
          : [...prev.permissionIds, permissionId],
      };
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    setSuccess('');
    setFormErrors({});

    try {
      const payload = {
        code: form.code.trim().toUpperCase(),
        name: form.name.trim(),
        description: form.description.trim(),
        permissionIds: form.permissionIds,
      };

      if (editing) {
        await roleApi.updateRole(editing.id, payload);
        setSuccess(`Đã cập nhật vai trò "${payload.name}".`);
      } else {
        await roleApi.createRole(payload);
        setSuccess(`Đã tạo vai trò "${payload.name}".`);
      }

      setFormOpen(false);
      await loadRoles();
    } catch (err) {
      // Backend trả về errors[] cho cả lỗi ModelState lẫn lỗi FluentValidation.
      const errors = err?.errors ?? [];
      if (errors.length > 0) {
        setError(errors.join(' '));
      } else {
        setError(err?.message || 'Không lưu được vai trò.');
      }

      // Gắn lỗi vào đúng trường để hiển thị dưới input nếu backend báo theo tên.
      const fieldMap = {};
      errors.forEach((msg) => {
        if (/mã vai trò/i.test(msg)) fieldMap.code = msg;
        else if (/tên vai trò/i.test(msg)) fieldMap.name = msg;
        else if (/mô tả/i.test(msg)) fieldMap.description = msg;
        else if (/quyền/i.test(msg)) fieldMap.permissionIds = msg;
      });
      setFormErrors(fieldMap);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (role) => {
    if (role.code === PROTECTED_ROLE_CODE) return;

    const confirmed = window.confirm(
      `Xóa vai trò "${role.name}"?\n\n` +
        'Các quyền hạn của vai trò này cũng sẽ bị gỡ khỏi mọi người dùng đang được gán vai trò này.'
    );
    if (!confirmed) return;

    setError('');
    setSuccess('');
    try {
      await roleApi.deleteRole(role.id);
      setSuccess(`Đã xóa vai trò "${role.name}".`);
      await loadRoles();
    } catch (err) {
      setError(err?.message || 'Không xóa được vai trò.');
    }
  };

  if (!canRead) {
    return (
      <div style={{ padding: '24px' }}>
        <Alert message="Bạn không có quyền xem danh sách vai trò (roles.read)." />
      </div>
    );
  }

  return (
    <div style={{ padding: '24px', maxWidth: '1100px', margin: '0 auto' }}>
      <h2 style={{ marginBottom: '20px' }}>Quản lý vai trò</h2>

      {error && <Alert message={error} />}
      {success && <Alert type="success" message={success} />}

      <Card
        title={`Danh sách vai trò (${roles.length})`}
        headerAction={
          canWrite && (
            <Button size="sm" onClick={openCreate}>
              + Thêm vai trò
            </Button>
          )
        }
      >
        {loading ? (
          <p style={{ color: 'var(--text-secondary)' }}>Đang tải...</p>
        ) : roles.length === 0 ? (
          <p style={{ color: 'var(--text-secondary)' }}>Chưa có vai trò nào.</p>
        ) : (
          <div style={{ overflowX: 'auto' }}>
            <table style={tableStyle}>
              <thead>
                <tr>
                  <th style={thStyle}>Mã</th>
                  <th style={thStyle}>Tên</th>
                  <th style={thStyle}>Mô tả</th>
                  <th style={thStyle}>Quyền hạn</th>
                  {canWrite && <th style={thStyle}>Thao tác</th>}
                </tr>
              </thead>
              <tbody>
                {roles.map((role) => (
                  <tr key={role.id}>
                    <td style={tdStyle}>
                      <strong>{role.code}</strong>
                    </td>
                    <td style={tdStyle}>{role.name}</td>
                    <td style={{ ...tdStyle, color: 'var(--text-secondary)' }}>
                      {role.description || '—'}
                    </td>
                    <td style={tdStyle}>
                      {(role.permissions || []).length === 0 ? (
                        <span style={{ color: 'var(--text-secondary)' }}>—</span>
                      ) : (
                        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                          {role.permissions.map((p) => (
                            <span
                              key={p.id}
                              className="badge badge-secondary"
                              title={p.description}
                            >
                              {p.code}
                            </span>
                          ))}
                        </div>
                      )}
                    </td>
                    {canWrite && (
                      <td style={{ ...tdStyle, whiteSpace: 'nowrap' }}>
                        <Button size="sm" variant="secondary" onClick={() => openEdit(role)}>
                          Sửa
                        </Button>{' '}
                        <Button
                          size="sm"
                          variant="danger"
                          onClick={() => handleDelete(role)}
                          disabled={role.code === PROTECTED_ROLE_CODE}
                          title={
                            role.code === PROTECTED_ROLE_CODE
                              ? 'Vai trò ADMIN không thể xóa'
                              : 'Xóa vai trò'
                          }
                        >
                          Xóa
                        </Button>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Card>

      <Modal
        isOpen={formOpen}
        onClose={() => setFormOpen(false)}
        title={editing ? `Sửa vai trò: ${editing.name}` : 'Thêm vai trò'}
        maxWidth="640px"
        footer={
          <>
            <Button variant="ghost" onClick={() => setFormOpen(false)} disabled={saving}>
              Hủy
            </Button>{' '}
            <Button onClick={handleSubmit} isLoading={saving}>
              {editing ? 'Lưu thay đổi' : 'Tạo vai trò'}
            </Button>
          </>
        }
      >
        <form onSubmit={handleSubmit}>
          <Input
            label="Mã vai trò"
            required
            value={form.code}
            onChange={(e) => setForm((p) => ({ ...p, code: e.target.value }))}
            placeholder="VD: EDITOR"
            error={formErrors.code}
            disabled={editing?.code === PROTECTED_ROLE_CODE}
          />
          <p
            style={{
              fontSize: '0.78rem',
              color: 'var(--text-secondary)',
              margin: '8px 0 14px',
            }}
          >
            {editing?.code === PROTECTED_ROLE_CODE
              ? 'Mã ADMIN là hệ thống nên không thể thay đổi.'
              : 'Chữ in hoa, số và dấu gạch dưới, bắt đầu bằng chữ cái. Tự động chuyển chữ hoa.'}
          </p>
          <Input
            label="Tên vai trò"
            required
            value={form.name}
            onChange={(e) => setForm((p) => ({ ...p, name: e.target.value }))}
            placeholder="VD: Biên tập viên"
            error={formErrors.name}
          />
          <Input
            label="Mô tả"
            value={form.description}
            onChange={(e) => setForm((p) => ({ ...p, description: e.target.value }))}
            placeholder="Mô tả ngắn về vai trò này"
            error={formErrors.description}
          />

          <div style={{ marginTop: '4px' }}>
            <label className="form-label">Quyền hạn</label>
            {formErrors.permissionIds && (
              <span className="form-error" style={{ display: 'block', marginBottom: '8px' }}>
                {formErrors.permissionIds}
              </span>
            )}

            {permissionsByModule.length === 0 ? (
              <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
                Chưa có quyền hạn nào trong hệ thống.
              </p>
            ) : (
              permissionsByModule.map(([module, permissions]) => (
                <fieldset
                  key={module}
                  style={{
                    border: '1px solid var(--border-color)',
                    borderRadius: 'var(--radius-md)',
                    padding: '10px 12px',
                    marginBottom: '10px',
                  }}
                >
                  <legend
                    style={{ fontSize: '0.8rem', fontWeight: 700, color: 'var(--text-secondary)' }}
                  >
                    {module}
                  </legend>
                  {permissions.map((p) => (
                    <label
                      key={p.id}
                      style={{
                        display: 'flex',
                        alignItems: 'center',
                        gap: '8px',
                        fontSize: '0.88rem',
                        color: 'var(--text-primary)',
                        padding: '3px 0',
                        cursor: 'pointer',
                      }}
                    >
                      <input
                        type="checkbox"
                        checked={form.permissionIds.includes(p.id)}
                        onChange={() => togglePermission(p.id)}
                      />
                      <span>
                        {p.name}{' '}
                        <span style={{ color: 'var(--text-secondary)' }}>({p.code})</span>
                      </span>
                    </label>
                  ))}
                </fieldset>
              ))
            )}
          </div>

          {/* Giữ nút submit của form để Enter hoạt động; nút chính nằm ở footer của Modal. */}
          <button type="submit" style={{ display: 'none' }} aria-hidden="true" />
        </form>
      </Modal>
    </div>
  );
};