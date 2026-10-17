import React from 'react';

export const Drawer = ({
  isOpen,
  onClose,
  title,
  children,
  footer,
  headerExtra,
  width = '880px',
}) => {
  if (!isOpen) return null;

  return (
    <div className="drawer-overlay" onClick={onClose}>
      <div
        className="drawer-panel"
        style={{ width }}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="drawer-header">
          <h3 className="drawer-title">{title}</h3>
          <div className="drawer-header-actions">
            {headerExtra}
            <button
              type="button"
              onClick={onClose}
              className="btn btn-ghost btn-sm drawer-close"
              aria-label="Đóng"
            >
              ✕
            </button>
          </div>
        </div>
        <div className="drawer-body">{children}</div>
        {footer && <div className="drawer-footer">{footer}</div>}
      </div>
    </div>
  );
};
