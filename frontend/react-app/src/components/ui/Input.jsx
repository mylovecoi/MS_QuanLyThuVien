import React, { useState } from 'react';
import { Eye, EyeOff } from 'lucide-react';

export const Input = ({ label, error, required = false, className = '', type = 'text', ...props }) => {
  const isPassword = type === 'password';
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="form-group">
      {label && (
        <label className="form-label">
          {label} {required && <span className="required">*</span>}
        </label>
      )}
      <div className={isPassword ? 'input-affix-wrapper' : ''}>
        <input
          className={`input-field ${error ? 'input-error' : ''} ${className}`}
          type={isPassword && showPassword ? 'text' : type}
          {...props}
        />
        {isPassword && (
          <button
            type="button"
            className="input-affix-btn"
            onClick={() => setShowPassword((prev) => !prev)}
            aria-label={showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'}
            tabIndex={-1}
          >
            {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
          </button>
        )}
      </div>
      {error && <span className="form-error">{error}</span>}
    </div>
  );
};
