import React from 'react';
import { Button } from './Button';

export const Modal = ({
  isOpen,
  onClose,
  title,
  children,
  footer,
  maxWidth = '600px',
}) => {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal-container"
        style={{ maxWidth }}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header">
          <h3 style={{ fontSize: '1.25rem', fontWeight: 700 }}>{title}</h3>
          <button
            onClick={onClose}
            className="btn btn-ghost btn-sm"
            style={{ fontSize: '1.25rem', padding: '4px 8px' }}
          >
            ✕
          </button>
        </div>
        <div className="modal-body">{children}</div>
        {footer && <div className="modal-footer">{footer}</div>}
      </div>
    </div>
  );
};
