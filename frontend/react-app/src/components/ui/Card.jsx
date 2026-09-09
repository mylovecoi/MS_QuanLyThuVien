import React from 'react';

export const Card = ({ children, className = '', title, headerAction, ...props }) => {
  return (
    <div className={`card ${className}`} {...props}>
      {title && (
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
          <h3 style={{ fontSize: '1.25rem', fontWeight: 700 }}>{title}</h3>
          {headerAction && <div>{headerAction}</div>}
        </div>
      )}
      {children}
    </div>
  );
};
