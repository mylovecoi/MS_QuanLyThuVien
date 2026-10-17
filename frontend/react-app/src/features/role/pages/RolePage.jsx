import React, { useCallback, useEffect, useState } from 'react';
import { roleApi } from '@/features/role/api/roleApi';
import { PermissionMatrix } from '@/features/role/components/PermissionMatrix';
import { useAuth } from '@/hooks/useAuth';
import { useToast } from '@/context/ToastContext';
import { Alert } from '@/components/feedback/Alert';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Drawer } from '@/components/ui/Drawer';
import { Input } from '@/components/ui/Input';
import { Modal } from '@/components/ui/Modal';
import { toErrorText } from '@/utils/apiError';
import { KeyRound, Pencil, Plus, RefreshCw, ShieldCheck, Trash2 } from 'lucide-react';

const PROTECTED_ROLE_CODE = 'ADMIN';
const EMPTY_FORM = { code: '', name: '', description: '' };

/** Lấy mảng permission ID từ role.permissions */
const idsOf = (role) => (role.permissions || []).map((p) => p.id);

export const RolePage = () => {
  const { user } = useAuth();
  const { showToast } = useToast();

  // ─── Quyền hạn của user hiện tại ───────────────────────────
  const canRead   = user?.permissions?.includes('roles.index');
  const canCreate = user?.permissions?.includes('roles.create');
  const canEdit   = user?.permissions?.includes('roles.edit');
  const canDelete = user?.permissions?.includes('roles.delete');

  // ─── State trang chính ──────────────────────────────────────
  const [roles, setRoles] = useState([]);
  /** Dữ liệu grouped từ /permissions/grouped — dùng cho Matrix UI */
  const [permissionGroups, setPermissionGroups] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // ─── State form thêm / sửa vai trò ──────────────────────────
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState(null);
  const [form, setForm] = useState(EMPTY_FORM);
  const [formSaving, setFormSaving] = useState(false);
  const [formErrors, setFormErrors] = useState({});

  // ─── State drawer ma trận phân quyền ────────────────────────
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [activeRole, setActiveRole] = useState(null);
  const [matrixSelection, setMatrixSelection] = useState([]);
  const [matrixSaving, setMatrixSaving] = useState(false);

  // ─── Load dữ liệu ───────────────────────────────────────────
  const loadRoles = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const [rolePage, groups] = await Promise.all([
        roleApi.getRoles(),
        roleApi.getPermissionsGrouped(),
      ]);
      setRoles(rolePage?.items ?? []);
      setPermissionGroups(groups ?? []);
    } catch (err) {
      setError(toErrorText(err, 'Không tải được danh sách vai trò.'));
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

  // Tổng số permission có thực (flat list, dùng để hiển thị badge đếm)
  const totalPermissions = permissionGroups.reduce(
    (sum, g) => sum + (g.permissions?.length ?? 0),
    0,
  );

  // ─── Ma trận phân quyền ──────────────────────────────────────
  const openMatrix = (role) => {
    setActiveRole(role);
    setMatrixSelection(idsOf(role));
    setError('');
    setDrawerOpen(true);
  };

  const togglePermission = (permissionId) => {
    setMatrixSelection((prev) => {
      const set = new Set(prev);
      if (set.has(permissionId)) set.delete(permissionId);
      else set.add(permissionId);
      return [...set];
    });
  };

  const toggleIds = (permissionIds, turnOn) => {
    setMatrixSelection((prev) => {
      const set = new Set(prev);
      permissionIds.forEach((id) => (turnOn ? set.add(id) : set.delete(id)));
      return [...set];
    });
  };

  const selectAll = (turnOn) => {
    const allIds = permissionGroups.flatMap((g) => (g.permissions || []).map((p) => p.id));
    setMatrixSelection(turnOn ? allIds : []);
  };

  const saveMatrix = async () => {
    if (!activeRole) return;
    setMatrixSaving(true);
    setError('');
    try {
      await roleApi.updateRole(activeRole.id, {
        code:          activeRole.code,
        name:          activeRole.name,
        description:   activeRole.description || '',
        permissionIds: matrixSelection,
      });
      showToast(`Đã lưu phân quyền cho vai trò "${activeRole.name}".`, { type: 'success' });
      setDrawerOpen(false);
      await loadRoles();
    } catch (err) {
      setError(toErrorText(err, 'Không lưu được ma trận phân quyền.'));
    } finally {
      setMatrixSaving(false);
    }
  };

  // ─── Thêm / sửa vai trò ─────────────────────────────────────
  const openCreate = () => {
    setEditing(null);
    setForm(EMPTY_FORM);
    setFormErrors({});
    setError('');
    setFormOpen(true);
  };

  const openEdit = (role) => {
    setEditing(role);
    setForm({ code: role.code, name: role.name, description: role.description || '' });
    setFormErrors({});
    setError('');
    setFormOpen(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setFormSaving(true);
    setError('');
    setFormErrors({});

    try {
      const payload = {
        code:          form.code.trim().toUpperCase(),
        name:          form.name.trim(),
        description:   form.description.trim(),
        permissionIds: editing ? idsOf(editing) : [],
      };

      if (editing) {
        await roleApi.updateRole(editing.id, payload);
        showToast(`Đã cập nhật vai trò "${payload.name}".`, { type: 'success' });
      } else {
        await roleApi.createRole(payload);
        showToast(`Đã tạo vai trò "${payload.name}". Hãy phân quyền rồi lưu.`, { type: 'success' });
      }

      setFormOpen(false);
      await loadRoles();
    } catch (err) {
      const text = toErrorText(err, 'Không lưu được vai trò.');
      setError(text);
      const fieldMap = {};
      if (/mã vai trò/i.test(text))  fieldMap.code        = text;
      else if (/tên vai trò/i.test(text)) fieldMap.name   = text;
      else if (/mô tả/i.test(text))  fieldMap.description = text;
      setFormErrors(fieldMap);
    } finally {
      setFormSaving(false);
    }
  };

  const handleDelete = async (role) => {
    if (role.code === PROTECTED_ROLE_CODE) return;

    const confirmed = window.confirm(
      `Xóa vai trò "${role.name}"?\n\n` +
        'Các quyền hạn của vai trò này cũng sẽ bị gỡ khỏi mọi người dùng đang được gán vai trò này.',
    );
    if (!confirmed) return;

    setError('');
    try {
      await roleApi.deleteRole(role.id);
      showToast(`Đã xóa vai trò "${role.name}".`, { type: 'success' });
      await loadRoles();
    } catch (err) {
      setError(toErrorText(err, 'Không xóa được vai trò.'));
    }
  };

  // ─── Guard: không có quyền xem ──────────────────────────────
  if (!canRead) {
    return (
      <div style={{ padding: '24px' }}>
        <Alert message="Bạn không có quyền xem danh sách vai trò (roles.index)." />
      </div>
    );
  }

  // ─── Render ─────────────────────────────────────────────────
  return (
    <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '24px 16px' }}>
      {/* Header */}
      <div className="card" style={{ marginBottom: 20, padding: '20px 24px' }}>
        <div
          style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            gap: 16,
            flexWrap: 'wrap',
          }}
        >
          <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
            <div
              style={{
                width: 44,
                height: 44,
                borderRadius: 12,
                background: 'var(--accent-light)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                flexShrink: 0,
              }}
            >
              <ShieldCheck size={22} color="var(--accent-color)" />
            </div>
            <div>
              <h1 style={{ margin: 0, fontSize: '1.25rem', fontWeight: 700 }}>
                Quản Lý Vai Trò &amp; Phân Quyền
              </h1>
              <p style={{ margin: '4px 0 0', color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
                Gán quyền theo ma trận [Xem, Tạo, Sửa, Xóa, Duyệt] cho từng vai trò.
              </p>
            </div>
          </div>
          <div style={{ display: 'flex', gap: 8 }}>
            <Button
              variant="secondary"
              size="sm"
              leftIcon={<RefreshCw size={16} />}
              onClick={loadRoles}
            >
              Tải lại
            </Button>
            {canCreate && (
              <Button size="sm" leftIcon={<Plus size={16} />} onClick={openCreate}>
                Thêm vai trò
              </Button>
            )}
          </div>
        </div>
      </div>

      <Alert message={error} />

      {/* Bảng danh sách vai trò */}
      <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
        <div className="perm-matrix-toolbar">
          <span style={{ fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
            {loading
              ? 'Đang tải...'
              : `${roles.length} vai trò · ${totalPermissions} quyền hạn`}
          </span>
        </div>

        <div style={{ overflowX: 'auto' }}>
          <table className="data-table" style={{ minWidth: 780 }}>
            <thead>
              <tr>
                <th>Vai trò</th>
                <th>Mô tả</th>
                <th style={{ textAlign: 'center' }}>Số quyền</th>
                <th style={{ textAlign: 'right' }}>Thao tác &amp; Phân quyền</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={4} className="data-table-empty">
                    Đang tải danh sách vai trò...
                  </td>
                </tr>
              ) : roles.length === 0 ? (
                <tr>
                  <td colSpan={4} className="data-table-empty">
                    Chưa có vai trò nào.
                  </td>
                </tr>
              ) : (
                roles.map((role) => {
                  const count       = (role.permissions || []).length;
                  const isProtected = role.code === PROTECTED_ROLE_CODE;

                  return (
                    <tr key={role.id}>
                      <td>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                          <div
                            style={{
                              width: 34,
                              height: 34,
                              borderRadius: 9,
                              background: 'var(--accent-light)',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              flexShrink: 0,
                            }}
                          >
                            <ShieldCheck size={17} color="var(--accent-color)" />
                          </div>
                          <div>
                            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                              <span style={{ fontWeight: 600 }}>{role.name}</span>
                              {isProtected && <Badge variant="warning">Hệ thống</Badge>}
                            </div>
                            <div
                              style={{
                                fontSize: '0.75rem',
                                color: 'var(--text-secondary)',
                                letterSpacing: '0.04em',
                              }}
                            >
                              {role.code}
                            </div>
                          </div>
                        </div>
                      </td>
                      <td style={{ color: 'var(--text-secondary)' }}>{role.description || '—'}</td>
                      <td style={{ textAlign: 'center' }}>
                        <Badge variant="primary">
                          {count} / {totalPermissions}
                        </Badge>
                      </td>
                      <td>
                        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 6 }}>
                          {canEdit && (
                            <Button
                              size="sm"
                              leftIcon={<KeyRound size={15} />}
                              onClick={() => openMatrix(role)}
                            >
                              Phân quyền
                            </Button>
                          )}
                          {canEdit && (
                            <Button
                              variant="ghost"
                              size="sm"
                              title="Chỉnh sửa thông tin"
                              onClick={() => openEdit(role)}
                            >
                              <Pencil size={16} />
                            </Button>
                          )}
                          {canDelete && (
                            <Button
                              variant="ghost"
                              size="sm"
                              title={
                                isProtected
                                  ? 'Vai trò ADMIN là hệ thống nên không thể xóa'
                                  : 'Xóa vai trò'
                              }
                              disabled={isProtected}
                              style={{ color: 'var(--error-color)' }}
                              onClick={() => handleDelete(role)}
                            >
                              <Trash2 size={16} />
                            </Button>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Drawer ma trận phân quyền */}
      <Drawer
        isOpen={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        width="920px"
        title={
          <>
            <KeyRound size={18} color="var(--accent-color)" />
            Ma trận phân quyền: <strong>{activeRole?.name}</strong>
          </>
        }
        headerExtra={
          canEdit && (
            <Button size="sm" isLoading={matrixSaving} onClick={saveMatrix}>
              Lưu phân quyền
            </Button>
          )
        }
      >
        <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', margin: '0 0 16px' }}>
          Click vào từng ô để cấp / gỡ quyền theo hành động của mỗi module.{' '}
          {canEdit
            ? 'Nút "Chọn tất cả" mỗi card giúp cấp nhanh toàn bộ quyền trong module.'
            : 'Bạn chỉ có quyền xem ma trận.'}
        </p>
        <PermissionMatrix
          permissionGroups={permissionGroups}
          selectedIds={matrixSelection}
          canWrite={canEdit}
          onTogglePermission={togglePermission}
          onToggleModule={toggleIds}
          onToggleAll={selectAll}
        />
      </Drawer>

      {/* Modal thêm / sửa vai trò */}
      <Modal
        isOpen={formOpen}
        onClose={() => setFormOpen(false)}
        title={editing ? `Sửa vai trò: ${editing.name}` : 'Thêm vai trò'}
        maxWidth="520px"
        footer={
          <>
            <Button variant="ghost" onClick={() => setFormOpen(false)} disabled={formSaving}>
              Hủy
            </Button>
            <Button onClick={handleSubmit} isLoading={formSaving}>
              {editing ? 'Lưu thông tin' : 'Tạo vai trò'}
            </Button>
          </>
        }
      >
        <form onSubmit={handleSubmit}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
            <div>
              <Input
                label="Mã vai trò"
                required
                value={form.code}
                onChange={(e) => setForm((p) => ({ ...p, code: e.target.value }))}
                placeholder="VD: EDITOR"
                error={formErrors.code}
                disabled={editing?.code === PROTECTED_ROLE_CODE}
              />
              <p style={{ fontSize: '0.78rem', color: 'var(--text-secondary)', margin: '6px 0 0' }}>
                {editing?.code === PROTECTED_ROLE_CODE
                  ? 'Mã ADMIN là hệ thống nên không thể thay đổi.'
                  : 'Chữ in hoa, số và dấu gạch dưới, bắt đầu bằng chữ cái. Tự động chuyển chữ hoa.'}
              </p>
            </div>
            <Input
              label="Tên vai trò"
              required
              value={form.name}
              onChange={(e) => setForm((p) => ({ ...p, name: e.target.value }))}
              placeholder="VD: Biên tập viên"
              error={formErrors.name}
            />
            <div className="form-group">
              <label className="form-label">Mô tả</label>
              <textarea
                className="textarea-field"
                rows={3}
                value={form.description}
                onChange={(e) => setForm((p) => ({ ...p, description: e.target.value }))}
                placeholder="Mô tả ngắn về vai trò này"
              />
              {formErrors.description && (
                <span className="form-error">{formErrors.description}</span>
              )}
            </div>
          </div>
          {!editing && (
            <p style={{ fontSize: '0.82rem', color: 'var(--text-secondary)', marginTop: 12 }}>
              Sau khi tạo, bấm <strong>Phân quyền</strong> để tick quyền rồi lưu.
            </p>
          )}
          <button type="submit" style={{ display: 'none' }} aria-hidden="true" />
        </form>
      </Modal>
    </div>
  );
};
