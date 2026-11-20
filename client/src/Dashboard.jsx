import React from 'react';
import { useAuth } from './context/AuthContext';

function Dashboard() {
  const { isAuthenticated, user, logout } = useAuth();

  if (!isAuthenticated || !user) {
    return (
      <div style={{ padding: '20px', textAlign: 'center' }}>
        <h3>⚠️ Доступ запрещён</h3>
        <p>Вы не авторизованы. Пожалуйста, войдите в систему.</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
      <header style={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center',
        marginBottom: '30px',
        padding: '20px',
        backgroundColor: '#f0f8ff',
        borderRadius: '8px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
      }}>
        <div>
          <h1 style={{ margin: 0, color: '#1a73e8' }}>🎵 Dashboard</h1>
          <p style={{ margin: '5px 0 0', color: '#666' }}>Личный кабинет пользователя</p>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
          <span style={{ fontSize: '14px', color: '#333' }}>
            👤 {user.username}
          </span>
          <button 
            className="logout-btn"
            onClick={logout}
            style={{
              padding: '10px 20px',
              backgroundColor: '#ff4444',
              color: 'white',
              border: 'none',
              borderRadius: '5px',
              cursor: 'pointer',
              fontSize: '14px'
            }}
          >
            Выйти
          </button>
        </div>
      </header>

      <main style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '20px' }}>
        {/* Welcome Card */}
        <div style={{ 
          padding: '25px',
          backgroundColor: '#ffffff',
          borderRadius: '8px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
          borderLeft: '4px solid #1a73e8'
        }}>
          <h2 style={{ margin: '0 0 15px', color: '#333' }}>👋 Добро пожаловать!</h2>
          <p style={{ color: '#666', lineHeight: '1.6' }}>
            Вы успешно авторизовались в системе SoundClowned. Здесь будет ваша личная панель управления музыкой, где вы сможете слушать треки, управлять плейлистами и взаимодействовать с сообществом.
          </p>
        </div>

        {/* Stats Card */}
        <div style={{ 
          padding: '25px',
          backgroundColor: '#ffffff',
          borderRadius: '8px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
          borderLeft: '4px solid #00c853'
        }}>
          <h2 style={{ margin: '0 0 15px', color: '#333' }}>📊 Статистика</h2>
          <div style={{ display: 'flex', justifyContent: 'space-around', alignItems: 'center' }}>
            <div style={{ textAlign: 'center' }}>
              <div style={{ fontSize: '48px', fontWeight: 'bold', color: '#1a73e8' }}>0</div>
              <div style={{ color: '#666', fontSize: '14px' }}>Треки</div>
            </div>
            <div style={{ textAlign: 'center' }}>
              <div style={{ fontSize: '48px', fontWeight: 'bold', color: '#00c853' }}>0</div>
              <div style={{ color: '#666', fontSize: '14px' }}>Плейлисты</div>
            </div>
            <div style={{ textAlign: 'center' }}>
              <div style={{ fontSize: '48px', fontWeight: 'bold', color: '#ff9800' }}>0</div>
              <div style={{ color: '#666', fontSize: '14px' }}>Часов прослушивания</div>
            </div>
          </div>
        </div>

        {/* Quick Actions Card */}
        <div style={{ 
          padding: '25px',
          backgroundColor: '#ffffff',
          borderRadius: '8px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
          borderLeft: '4px solid #ff9800'
        }}>
          <h2 style={{ margin: '0 0 15px', color: '#333' }}>⚡ Быстрые действия</h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
            <button 
              style={{
                padding: '12px 20px',
                backgroundColor: '#e8f0fe',
                color: '#1a73e8',
                border: 'none',
                borderRadius: '5px',
                cursor: 'pointer',
                fontSize: '14px',
                fontWeight: '500'
              }}
            >
              🎵 Добавить трек в плейлист
            </button>
            <button 
              style={{
                padding: '12px 20px',
                backgroundColor: '#e8f5e9',
                color: '#00c853',
                border: 'none',
                borderRadius: '5px',
                cursor: 'pointer',
                fontSize: '14px',
                fontWeight: '500'
              }}
            >
              📁 Создать новый плейлист
            </button>
            <button 
              style={{
                padding: '12px 20px',
                backgroundColor: '#fff3e0',
                color: '#ff9800',
                border: 'none',
                borderRadius: '5px',
                cursor: 'pointer',
                fontSize: '14px',
                fontWeight: '500'
              }}
            >
              🎧 Настроить аудиоустройства
            </button>
          </div>
        </div>

        {/* Info Card */}
        <div style={{ 
          padding: '25px',
          backgroundColor: '#ffffff',
          borderRadius: '8px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
          borderLeft: '4px solid #9c27b0'
        }}>
          <h2 style={{ margin: '0 0 15px', color: '#333' }}>ℹ️ Информация</h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
            <p style={{ color: '#666', margin: 0 }}>
              <strong>Email:</strong> {user.email || user.username}
            </p>
            <p style={{ color: '#666', margin: 0 }}>
              <strong>ID пользователя:</strong> {user.id || 'N/A'}
            </p>
            <p style={{ color: '#666', margin: 0 }}>
              <strong>Роль:</strong> {user.role || 'User'}
            </p>
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer style={{ 
        marginTop: '30px', 
        padding: '20px', 
        textAlign: 'center',
        backgroundColor: '#f5f5f5',
        borderRadius: '8px'
      }}>
        <p style={{ margin: 0, color: '#666' }}>
          © 2024 SoundClowned. Все права защищены.
        </p>
      </footer>
    </div>
  );
}

export default Dashboard;
