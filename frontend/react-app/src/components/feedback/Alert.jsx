import React from 'react';

export const Alert = ({ type = 'error', message }) => {
  if (!message) return null;
  const isError = type === 'error';
  return (
    <div style={{
      padding: '12px 16px',
      borderRadius: 'var(--radius-md)',
      backgroundColor: isError ? 'rgba(239, 68, 68, 0.15)' : 'rgba(16, 185, 129, 0.15)',
      color: isError ? 'var(--error-color)' : 'var(--success-color)',
      border: `1px solid ${isError ? 'var(--error-color)' : 'var(--success-color)'}`,
      marginBottom: '16px',
      fontSize: '1rem'
    }}>
      {message}
    </div>
  );
};
