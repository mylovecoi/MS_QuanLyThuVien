import React from 'react';

export const Select = ({
  label,
  options = [],
  error,
  required = false,
  placeholder = '-- Chọn --',
  className = '',
  ...props
}) => {
  return (
    <div className="form-group">
      {label && (
        <label className="form-label">
          {label} {required && <span className="required">*</span>}
        </label>
      )}
      <select
        className={`select-field ${error ? 'input-error' : ''} ${className}`}
        {...props}
      >
        {placeholder && <option value="">{placeholder}</option>}
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
      {error && <span className="form-error">{error}</span>}
    </div>
  );
};
