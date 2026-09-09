import React from 'react';

export const Input = ({ label, error, required = false, className = '', ...props }) => {
  return (
    <div className="form-group">
      {label && (
        <label className="form-label">
          {label} {required && <span className="required">*</span>}
        </label>
      )}
      <input
        className={`input-field ${error ? 'input-error' : ''} ${className}`}
        {...props}
      />
      {error && <span className="form-error">{error}</span>}
    </div>
  );
};
