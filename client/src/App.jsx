import React, { useState, useEffect } from 'react';
import LandingPage from './LandingPage';
import AuthModal from './AuthModal';
import Dashboard from './Dashboard';
import { AuthProvider, useAuth } from './context/AuthContext';

function AppContent() {
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [currentHash, setCurrentHash] = useState(window.location.hash);
  const auth = useAuth();

  useEffect(() => {
    const handleOpenAuth = () => {
      setIsAuthModalOpen(true);
    };

    const handleHashChange = () => {
      setCurrentHash(window.location.hash);
    };

    window.addEventListener('openAuth', handleOpenAuth);
    window.addEventListener('hashchange', handleHashChange);
    
    return () => {
      window.removeEventListener('openAuth', handleOpenAuth);
      window.removeEventListener('hashchange', handleHashChange);
    };
  }, []);

  if (auth.isAuthenticated) {
    if (currentHash === '#/dashboard') {
      return (
        <div className="App">
          <Dashboard />
          <AuthModal 
            isOpen={isAuthModalOpen}
            onClose={() => setIsAuthModalOpen(false)}
          />
        </div>
      );
    }

    return (
      <div className="App">
        <h1 className="app-welcome">Добро пожаловать, {auth.user?.username}!</h1>
        <LandingPage onAuthClose={() => setIsAuthModalOpen(false)} />
        <div className="auth-section">
          <button 
            className="logout-btn" 
            onClick={() => {
              auth.logout();
              window.location.hash = '/';
            }}
          >
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
