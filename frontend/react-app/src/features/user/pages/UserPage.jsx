import React, { useCallback, useEffect, useState } from 'react';
import { userApi } from '@/features/user/api/userApi';
import { roleApi } from '@/features/role/api/roleApi';
import { useAuth } from '@/hooks/useAuth';
import { useToast } from '@/context/ToastContext';
import { Alert } from '@/components/feedback/Alert';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { UserFormModal } from '@/features/user/components/UserFormModal';
import { ResetPasswordModal } from '@/features/user/components/ResetPasswordModal';
import { ConfirmModal } from '@/components/ui/ConfirmModal';
import { toErrorText } from '@/utils/apiError';
import { ACCOUNT_STATUS_OPTIONS } from '@/utils/constants';
import { KeyRound, Lock, LockOpen, Pencil, Plus, Search, Trash2, Users } from 'lucide-react';

const PAGE_SIZE = 20;

const getStatusText = (value) => ACCOUNT_STATUS_OPTIONS.find((s) => s.value === value)?.label || '—';
const getStatusVariant = (value) => {
  if (value === 1) return 'success'; // Active
  if (value === 2) return 'warning'; // Locked
  if (value === 3) return 'danger';  // Disabled
  return 'secondary';                // Pending
};

export const UserPage = () => {
  const { user: currentUser } = useAuth();
  const { showToast } = useToast();

  const canRead = currentUser?.permissions?.includes('users.index');
  const canCreate = currentUser?.permissions?.includes('users.create');
  const canEdit = currentUser?.permissions?.includes('users.edit');
  const canDelete = currentUser?.permissions?.includes('users.delete');

  const [users, setUsers] = useState([]);
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [search, setSearch] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState(null);
  const [formSaving, setFormSaving] = useState(false);
  const [formErrors, setFormErrors] = useState({});

  const [pwdOpen, setPwdOpen] = useState(false);
  const [pwdTarget, setPwdTarget] = useState(null);
  const [pwdSaving, setPwdSaving] = useState(false);
  const [pwdError, setPwdError] = useState('');

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const loadUsers = useCallback(async () => {
    if (!canRead) return;
    setLoading(true);
    setError('');
    try {
      const res = await userApi.getUsers(page, PAGE_SIZE, searchTerm);
      setUsers(res?.items ?? []);
      setTotalCount(res?.totalCount ?? 0);
      setTotalPages(res?.totalPages ?? 0);
    } catch (err) {
      setError(toErrorText(err, 'Không tải được danh sách tài khoản.'));
    } finally {
      setLoading(false);
    }
  }, [canRead, page, searchTerm]);

  const loadRoles = useCallback(async () => {
    try {
      const rolePage = await roleApi.getRoles();
      setRoles(rolePage?.items ?? []);
    } catch (err) {
      // Không chặn hiển thị form nếu roles lỗi, chỉ log qua UI nếu cần.
    }
  }, []);

  useEffect(() => {
    if (canRead) {
      loadUsers();
    } else {
      setLoading(false);
    }
  }, [canRead, loadUsers]);

  useEffect(() => {
    if (canRead) {
      loadRoles();
    }
  }, [canRead, loadRoles]);

  const handleSearchSubmit = (e) => {
    if (e) e.preventDefault();
    setPage(1);
    setSearchTerm(search.trim());
  };

  const handleClearSearch = () => {
    setSearch('');
    setPage(1);
    setSearchTerm('');
  };

  const openCreate = () => {
    setEditing(null);
    setFormErrors({});
    setError('');
    setFormOpen(true);
  };

  const openEdit = (u) => {
    setEditing(u);
    setFormErrors({});
    setError('');
    setFormOpen(true);
  };

  const handleFormSubmit = async (payload) => {
    setFormSaving(true);
    setFormErrors({});
    setError('');
    try {
      if (editing) {
        await userApi.updateUser(editing.id, payload);
        showToast(`Đã cập nhật tài khoản "${payload.fullName}".`, { type: 'success' });
      } else {
        await userApi.createUser(payload);
        showToast(`Đã tạo tài khoản "${payload.username}".`, { type: 'success' });
      }
      setFormOpen(false);
      await loadUsers();
    } catch (err) {
      const text = toErrorText(err, 'Không lưu được tài khoản.');
      setError(text);
      const fieldMap = {};
      if (/tên đăng nhập/i.test(text)) fieldMap.username = text;
      else if (/email/i.test(text)) fieldMap.email = text;
      else if (/họ và tên/i.test(text)) fieldMap.fullName = text;
      else if (/mật khẩu/i.test(text)) fieldMap.password = text;
      else if (/vai trò/i.test(text)) fieldMap.roleIds = text;
      setFormErrors(fieldMap);
    } finally {
      setFormSaving(false);
    }
  };

  const openResetPwd = (u) => {
    setPwdTarget(u);
    setPwdError('');
    setPwdOpen(true);
  };

  const handleResetPwd = async (newPassword) => {
    if (!pwdTarget) return;
    setPwdSaving(true);
    setPwdError('');
    try {
      await userApi.resetPassword(pwdTarget.id, newPassword);
      showToast(`Đã đặt lại mật khẩu tài khoản "${pwdTarget.username}".`, { type: 'success' });
      setPwdOpen(false);
    } catch (err) {
      setPwdError(toErrorText(err, 'Không đặt lại được mật khẩu.'));
    } finally {
      setPwdSaving(false);
    }
  };

  const handleToggleStatus = async (u) => {
    const targetStatus = u.status === 2 ? 1 : 2; // 2=Locked -> Active, else Lock
    const label = targetStatus === 2 ? 'khóa' : 'mở khóa';
    try {
      await userApi.updateStatus(u.id, targetStatus);
      showToast(`Đã ${label} tài khoản "${u.username}".`, { type: 'success' });
      await loadUsers();
    } catch (err) {
      setError(toErrorText(err, `Không ${label} được tài khoản.`));
    }
  };

  const handleDelete = async (u) => {
    if (u.id === currentUser?.id) return;
    setDeleteTarget(u);
    setDeleteOpen(true);
  };

  const confirmDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    setError('');
    try {
      await userApi.deleteUser(deleteTarget.id);
      showToast(`Đã xóa tài khoản "${deleteTarget.username}".`, { type: 'success' });
      setDeleteOpen(false);
      setDeleteTarget(null);
      await loadUsers();
    } catch (err) {
      setError(toErrorText(err, 'Không xóa được tài khoản.'));
    } finally {
      setDeleting(false);
    }
  };

  if (!canRead) {
    return (
      <div style={{ padding: '24px' }}>
        <Alert message="Bạn không có quyền xem danh sách tài khoản (users.index)." />
      </div>
    );
  }

  return (
    <div style={{ margin: '0 auto', padding: '24px 16px' }}>
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
              <Users size={22} color="var(--accent-color)" />
            </div>
            <div>
              <h1 style={{ margin: 0, fontSize: '1.25rem', fontWeight: 700 }}>Quản Lý Tài Khoản</h1>
              <p style={{ margin: '4px 0 0', color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
                Tìm kiếm, tạo, chỉnh sửa vai trò, khóa/mở và đặt lại mật khẩu tài khoản.
              </p>
            </div>
          </div>
          <div style={{ display: 'flex', gap: 8 }}>
            {canCreate && (
              <Button size="sm" leftIcon={<Plus size={16} />} onClick={openCreate}>
                Thêm tài khoản
              </Button>
            )}
          </div>
        </div>

        <form
          onSubmit={handleSearchSubmit}
          style={{
            marginTop: 16,
            display: 'flex',
            gap: 8,
            flexWrap: 'wrap',
            alignItems: 'center',
          }}
        >
          <div style={{ flex: '1 1 320px', maxWidth: 420 }}>
            <Input
              placeholder="Tìm theo username, email, họ tên..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <Button size="sm" leftIcon={<Search size={16} />} type="submit">
            Tìm kiếm
          </Button>
          {searchTerm && (
            <Button size="sm" variant="ghost" onClick={handleClearSearch}>
              Xóa bộ lọc
            </Button>
          )}
        </form>
      </div>

      <Alert message={error} />

      <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
        <div className="perm-matrix-toolbar">
          <span style={{ fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
            {loading
              ? 'Đang tải...'
              : `${totalCount} tài khoản · Trang ${page}/${totalPages || 1}`}
          </span>
        </div>

        <div style={{ overflowX: 'auto' }}>
          <table className="data-table" style={{ minWidth: 880 }}>
            <thead>
              <tr>
                <th>Tài khoản</th>
                <th>Email</th>
                <th>Họ và tên</th>
                <th>Vai trò</th>
                <th>Trạng thái</th>
                <th>Ngày tạo</th>
                <th style={{ textAlign: 'right' }}>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={8} className="data-table-empty">
                    Đang tải danh sách tài khoản...
                  </td>
                </tr>
              ) : users.length === 0 ? (
                <tr>
                  <td colSpan={8} className="data-table-empty">
                    Chưa có tài khoản nào.
                  </td>
                </tr>
              ) : (
                users.map((u) => {
                  const isSelf = u.id === currentUser?.id;
                  return (
                    <tr key={u.id}>
                      <td>
                        <div>
                          <span style={{ fontWeight: 600 }}>{u.username}</span>
                        </div>
                      </td>
                      <td>{u.email}</td>
                      <td>{u.fullName}</td>
                      <td>
                        <div style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}>
                          {(u.roles || []).length === 0 ? (
                            <span style={{ color: 'var(--text-secondary)' }}>—</span>
                          ) : (
                            (u.roles || []).map((r) => (
                              <Badge key={r.id} variant="primary">
                                {r.name}
                              </Badge>
                            ))
                          )}
                        </div>
                      </td>
                      <td>
                        <Badge variant={getStatusVariant(u.status)}>{getStatusText(u.status)}</Badge>
                      </td>
                      <td style={{ color: 'var(--text-secondary)' }}>
                        {new Date(u.createdAt).toLocaleDateString('vi-VN')}
                      </td>
                      <td>
                        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 6, flexWrap: 'wrap' }}>
                          {canEdit && (
                            <Button
                              variant="ghost"
                              size="sm"
                              title="Đặt lại mật khẩu"
                              onClick={() => openResetPwd(u)}
                            >
                              <KeyRound size={16} />
                            </Button>
                          )}
                          {canEdit && (
                            <Button
                              variant="ghost"
                              size="sm"
                              title="Chỉnh sửa"
                              onClick={() => openEdit(u)}
                            >
                              <Pencil size={16} />
                            </Button>
                          )}
                          {canEdit && (
                            <Button
                              variant="ghost"
                              size="sm"
                              title={u.status === 2 ? 'Mở khóa' : 'Khóa tài khoản'}
                              onClick={() => handleToggleStatus(u)}
                              disabled={isSelf}
                              style={{ color: u.status === 2 ? 'var(--success-color)' : 'var(--warning-color)' }}
                            >
                              {u.status === 2 ? <LockOpen size={16} /> : <Lock size={16} />}
                            </Button>
                          )}
                          {canDelete && (
                            <Button
                              variant="ghost"
                              size="sm"
                              title={isSelf ? 'Không thể xóa tài khoản của chính mình' : 'Xóa tài khoản'}
                              onClick={() => handleDelete(u)}
                              disabled={isSelf}
                              style={{ color: 'var(--error-color)' }}
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

        {totalPages > 1 && (
          <div
            style={{
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              padding: '12px 16px',
              borderTop: '1px solid var(--border-color)',
              flexWrap: 'wrap',
              gap: 8,
            }}
          >
            <span style={{ fontSize: '0.9rem', color: 'var(--text-secondary)' }}>
              Trang {page} / {totalPages}
            </span>
            <div style={{ display: 'flex', gap: 8 }}>
              <Button
                size="sm"
                variant="ghost"
                disabled={page <= 1}
                onClick={() => setPage((p) => Math.max(1, p - 1))}
              >
                Trước
              </Button>
              <Button
                size="sm"
                variant="ghost"
                disabled={page >= totalPages}
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              >
                Sau
              </Button>
            </div>
          </div>
        )}
      </div>

      <UserFormModal
        isOpen={formOpen}
        onClose={() => setFormOpen(false)}
        editing={editing}
        roles={roles}
        saving={formSaving}
        errors={formErrors}
        onSubmit={handleFormSubmit}
      />

      <ResetPasswordModal
        isOpen={pwdOpen}
        onClose={() => setPwdOpen(false)}
        username={pwdTarget?.username}
        saving={pwdSaving}
        error={pwdError}
        onSubmit={handleResetPwd}
      />

      <ConfirmModal
        isOpen={deleteOpen}
        onClose={() => setDeleteOpen(false)}
        onConfirm={confirmDelete}
        loading={deleting}
        title="Xóa tài khoản"
        message={deleteTarget ? `Xóa tài khoản "${deleteTarget.username}"?\n\nTài khoản này sẽ bị xoá mềm và không thể đăng nhập.` : ''}
        confirmText="Xóa"
        variant="danger"
      />
    </div>
  );
};
