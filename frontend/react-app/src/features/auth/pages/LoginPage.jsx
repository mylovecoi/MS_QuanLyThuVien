import React from 'react';
import { LoginForm } from '../components/LoginForm';

export const LoginPage = () => {
  return (
    <div style={{
      minHeight: 'calc(100vh - 64px)',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '24px'
    }}>
      <LoginForm />
    </div>
  );
};
