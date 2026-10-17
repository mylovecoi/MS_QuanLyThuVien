import React, { useMemo } from 'react';
import {
  Eye,
  Plus,
  Pencil,
  Trash2,
  CheckCircle,
  BookOpen,
  Archive,
  ArrowLeftRight,
  ShoppingCart,
  Star,
  Bell,
  Search,
  BarChart2,
  Users,
  ShieldCheck,
} from 'lucide-react';

// ─────────────────────────────────────────
// Cấu hình 5 action chuẩn
// ─────────────────────────────────────────
const ACTIONS = ['index', 'create', 'edit', 'delete', 'approve'];

const ACTION_META = {
  index:   { label: 'Xem DS',    Icon: Eye           },
  create:  { label: 'Tạo mới',   Icon: Plus          },
  edit:    { label: 'Chỉnh sửa', Icon: Pencil        },
  delete:  { label: 'Xóa',       Icon: Trash2        },
  approve: { label: 'Duyệt',     Icon: CheckCircle   },
};

// ─────────────────────────────────────────
// Metadata từng module: nhãn + icon
// ─────────────────────────────────────────
const MODULE_META = {
  Catalog:      { label: 'Danh mục thư mục',    Icon: BookOpen       },
  Inventory:    { label: 'Kho & Bản sao',        Icon: Archive        },
  Circulation:  { label: 'Mượn trả sách',        Icon: ArrowLeftRight },
  Acquisition:  { label: 'Bổ sung tài liệu',     Icon: ShoppingCart   },
  Review:       { label: 'Đánh giá & Bình luận', Icon: Star           },
  Notification: { label: 'Thông báo',            Icon: Bell           },
  Search:       { label: 'Tìm kiếm',             Icon: Search         },
  Reporting:    { label: 'Báo cáo & Thống kê',   Icon: BarChart2      },
  Users:        { label: 'Người dùng',           Icon: Users          },
  Roles:        { label: 'Vai trò & Phân quyền', Icon: ShieldCheck    },
};

// ─────────────────────────────────────────
// Helper: tạo lookup Map { "Module::action" → PermissionResponse }
// ─────────────────────────────────────────
const buildLookup = (groups) => {
  const map = new Map();
  (groups || []).forEach((g) => {
    (g.permissions || []).forEach((p) => {
      const action = p.code.includes('.') ? p.code.split('.').pop() : p.code;
      map.set(`${g.module}::${action}`, p);
    });
  });
  return map;
};

// ─────────────────────────────────────────
// Sub-component: 1 ô action
// ─────────────────────────────────────────
const ActionCell = ({ perm, action, checked, canWrite, onToggle }) => {
  const meta = ACTION_META[action] ?? { label: action, Icon: Eye };
  const { Icon } = meta;

  // Permission không tồn tại trong module này
  if (!perm) {
    return (
      <div className="pm-action-cell is-na" data-action={action} title={`${meta.label} — không áp dụng`}>
        <div className="pm-action-icon">
          <Icon size={15} />
        </div>
        <span className="pm-action-label">{meta.label}</span>
        <span className="pm-action-na-dash">—</span>
      </div>
    );
  }

  return (
    <div
      className={`pm-action-cell${checked ? ' is-on' : ''}${!canWrite ? ' is-readonly' : ''}`}
      data-action={action}
      title={`${perm.name}${perm.description ? ` — ${perm.description}` : ''}`}
      onClick={canWrite ? () => onToggle(perm.id) : undefined}
      role={canWrite ? 'checkbox' : undefined}
      aria-checked={canWrite ? checked : undefined}
      tabIndex={canWrite ? 0 : undefined}
      onKeyDown={canWrite ? (e) => { if (e.key === ' ' || e.key === 'Enter') { e.preventDefault(); onToggle(perm.id); } } : undefined}
    >
      <div className="pm-action-icon">
        <Icon size={16} />
      </div>
      <span className="pm-action-label">{meta.label}</span>
      {canWrite && (
        <input
          type="checkbox"
          className="pm-action-check"
          checked={checked}
          readOnly
          tabIndex={-1}
          aria-hidden="true"
        />
      )}
    </div>
  );
};

// ─────────────────────────────────────────
// Sub-component: 1 module card
// ─────────────────────────────────────────
const ModuleCard = ({ group, lookup, selected, canWrite, onTogglePermission, onToggleModule }) => {
  const { module, moduleLabel } = group;
  const meta = MODULE_META[module] ?? { label: moduleLabel || module, Icon: ShieldCheck };
  const { Icon } = meta;

  // IDs thực sự tồn tại trong module này
  const existingIds = ACTIONS
    .map((a) => lookup.get(`${module}::${a}`)?.id)
    .filter(Boolean);

  const moduleOn   = existingIds.length > 0 && existingIds.every((id) => selected.has(id));
  const moduleSome = existingIds.some((id) => selected.has(id));

  const handleSelectAll = (e) => {
    e.stopPropagation();
    onToggleModule(existingIds, !moduleOn);
  };

  return (
    <div className="pm-card">
      {/* Header card */}
      <div className="pm-card-header">
        <div className="pm-card-icon">
          <Icon size={18} />
        </div>
        <div className="pm-card-info">
          <div className="pm-card-title">{meta.label !== module ? meta.label : moduleLabel}</div>
          <div className="pm-card-sub">{module}</div>
        </div>

        {/* Select-all toggle */}
        <label
          className={[
            'pm-select-all',
            moduleOn        ? 'is-active'   : '',
            !canWrite       ? 'is-disabled' : '',
          ].join(' ')}
          title={canWrite ? `Chọn / bỏ chọn tất cả quyền của module ${meta.label}` : 'Chỉ xem'}
          onClick={canWrite ? handleSelectAll : undefined}
        >
          <input
            type="checkbox"
            checked={moduleOn}
            ref={(el) => { if (el) el.indeterminate = moduleSome && !moduleOn; }}
            readOnly
            tabIndex={-1}
            aria-hidden="true"
          />
          Chọn tất cả
        </label>
      </div>

      {/* Grid 5 action cells */}
      <div className="pm-action-grid">
        {ACTIONS.map((action) => {
          const perm    = lookup.get(`${module}::${action}`);
          const checked = perm ? selected.has(perm.id) : false;
          return (
            <ActionCell
              key={action}
              perm={perm}
              action={action}
              checked={checked}
              canWrite={canWrite}
              onToggle={onTogglePermission}
            />
          );
        })}
      </div>
    </div>
  );
};

// ─────────────────────────────────────────
// Export chính: PermissionMatrix
// ─────────────────────────────────────────
/**
 * @param {object[]} permissionGroups  - Mảng PermissionGroupResponse từ API /permissions/grouped
 * @param {string[]} selectedIds       - Mảng ID permission đang được chọn
 * @param {boolean}  canWrite          - Có quyền chỉnh sửa không
 * @param {Function} onTogglePermission(id)          - Toggle 1 permission
 * @param {Function} onToggleModule(ids, turnOn)     - Toggle cả module
 * @param {Function} onToggleAll(turnOn)             - Toggle tất cả
 */
export const PermissionMatrix = ({
  permissionGroups,
  selectedIds,
  canWrite,
  onTogglePermission,
  onToggleModule,
  onToggleAll,
}) => {
  const lookup   = useMemo(() => buildLookup(permissionGroups), [permissionGroups]);
  const selected = useMemo(() => new Set(selectedIds || []),    [selectedIds]);

  if (!permissionGroups?.length) {
    return (
      <p style={{ color: 'var(--text-secondary)', padding: '24px 0' }}>
        Chưa có quyền hạn nào trong hệ thống.
      </p>
    );
  }

  // Tổng số permissions có thực (bỏ các ô N/A)
  const allIds  = [...lookup.values()].map((p) => p.id);
  const allOn   = allIds.length > 0 && allIds.every((id) => selected.has(id));
  const someOn  = allIds.some((id) => selected.has(id));
  const countOn = allIds.filter((id) => selected.has(id)).length;

  return (
    <div>
      {/* Summary + Select All toàn bộ */}
      <div className="pm-summary-bar">
        <span>
          <span className="pm-summary-count">{countOn}</span>
          {' / '}
          {allIds.length} quyền được cấp
        </span>
        {canWrite && (
          <label
            className={['pm-select-all', allOn ? 'is-active' : ''].join(' ')}
            style={{ marginLeft: 'auto' }}
            onClick={() => onToggleAll(!allOn)}
            title="Cấp / gỡ toàn bộ quyền"
          >
            <input
              type="checkbox"
              checked={allOn}
              ref={(el) => { if (el) el.indeterminate = someOn && !allOn; }}
              readOnly
              tabIndex={-1}
              aria-hidden="true"
            />
            Tất cả
          </label>
        )}
      </div>

      {/* Danh sách module cards */}
      <div className="pm-card-list">
        {permissionGroups.map((group) => (
          <ModuleCard
            key={group.module}
            group={group}
            lookup={lookup}
            selected={selected}
            canWrite={canWrite}
            onTogglePermission={onTogglePermission}
            onToggleModule={onToggleModule}
          />
        ))}
      </div>
    </div>
  );
};

// Giữ lại export cũ để không break import nào đang dùng buildMatrixModel
export const buildMatrixModel = (permissions) => {
  const actions = [];
  const seenActions = new Set();
  const modules = [];
  const seenModules = new Set();
  const byKey = new Map();

  (permissions || []).forEach((p) => {
    const module = p.module || 'Khác';
    const action = p.code.includes('.') ? p.code.split('.').pop() : p.code;
    if (!seenModules.has(module)) { seenModules.add(module); modules.push(module); }
    if (!seenActions.has(action)) { seenActions.add(action); actions.push(action); }
    byKey.set(`${module}::${action}`, p);
  });

  const order = ['index', 'create', 'edit', 'delete', 'approve'];
  actions.sort((a, b) => {
    const ia = order.indexOf(a), ib = order.indexOf(b);
    if (ia === -1 && ib === -1) return a.localeCompare(b);
    if (ia === -1) return 1; if (ib === -1) return -1;
    return ia - ib;
  });

  return { modules, actions, byKey };
};
