import React, { useState } from 'react';
import LandingPage from './LandingPage';
import AuthModal from './AuthModal';
import { AuthProvider, useAuth } from './context/AuthContext';

function AppContent() {
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const auth = useAuth();

  // Обработка события от кнопки "Войти на сайт"
  React.useEffect(() => {
    const handleOpenAuth = () => {
      console.log('Открываю модальное окно авторизации');
      setIsAuthModalOpen(true);
    };

    window.addEventListener('openAuth', handleOpenAuth);
    
    return () => {
      window.removeEventListener('openAuth', handleOpenAuth);
    };
  }, []);

  // Кнопка "Выход" после авторизации
  if (auth.isAuthenticated) {
    return (
      <div className="App">
        <h1 className="app-welcome">Добро пожаловать, {auth.user?.username}!</h1>
        <LandingPage onAuthClose={() => setIsAuthModalOpen(false)} />
        <div className="auth-section">
          <button className="logout-btn" onClick={() => auth.logout()}>
            Выйти
          </button>
        </div>
        <AuthModal 
          isOpen={isAuthModalOpen}
          onClose={() => setIsAuthModalOpen(false)}
        />
      </div>
    );
  }

  return (
    <div className="App">
      <LandingPage onAuthClose={() => setIsAuthModalOpen(false)} />
      <AuthModal 
        isOpen={isAuthModalOpen}
        onClose={() => setIsAuthModalOpen(false)}
      />
    </div>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export default App;
