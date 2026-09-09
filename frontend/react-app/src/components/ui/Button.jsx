import React from 'react';

export const Button = ({
  children,
  variant = 'primary',
  size = 'md',
  className = '',
  isLoading = false,
  disabled = false,
  leftIcon,
  rightIcon,
  ...props
}) => {
  const sizeClass = size !== 'md' ? `btn-${size}` : '';
  const variantClass = `btn-${variant}`;

  return (
    <button
      className={`btn ${variantClass} ${sizeClass} ${className}`}
      disabled={isLoading || disabled}
      {...props}
    >
      {isLoading ? (
        <>
          <span className="btn-spinner" />
          <span>Đang xử lý...</span>
        </>
      ) : (
        <>
          {leftIcon && <span className="btn-icon">{leftIcon}</span>}
          {children}
          {rightIcon && <span className="btn-icon">{rightIcon}</span>}
        </>
      )}
    </button>
  );
};
