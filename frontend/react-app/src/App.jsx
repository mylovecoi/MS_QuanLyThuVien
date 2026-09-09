import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ThemeProvider } from './context/ThemeContext';
import { AppRoutes } from './routes/AppRoutes';
import { Header } from './components/layout/Header';
import './assets/styles.css';

export function App() {
  return (
    <BrowserRouter>
      <ThemeProvider>
        <AuthProvider>
          <Header />
          <main>
            <AppRoutes />
          </main>
        </AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
}

export default App;
