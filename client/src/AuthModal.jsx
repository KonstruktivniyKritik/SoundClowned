import React from 'react';
import { useAuth } from './context/AuthContext';
import './AuthModal.css';

const API_URL = 'http://localhost:8081/api/auth';

const AuthModal = ({ isOpen, onClose }) => {
  const { login } = useAuth();
  const [isRegisterMode, setIsRegisterMode] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const [error, setError] = React.useState('');
  const [success, setSuccess] = React.useState('');

  React.useEffect(() => {
    setError('');
    setSuccess('');
    setIsRegisterMode(false);
  }, [isOpen]);

  React.useEffect(() => {
    const handleKeyDown = (e) => {
      if (e.key === 'Escape' && isOpen && onClose) {
        onClose();
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    const formData = new FormData(e.target);
    const data = {
      email: formData.get('email'),
      password: formData.get('password'),
      fullName: formData.get('fullName') || ''
    };

    setIsLoading(true);

    try {
      if (isRegisterMode) {
        const response = await fetch(`${API_URL}/register`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            username: data.fullName,
            email: data.email,
            password: data.password
          })
        });

        if (!response.ok) {
          const errorText = await response.text();
          let errorMessage = 'Ошибка регистрации';

          if (errorText) {
            try {
              const errData = JSON.parse(errorText);
              if (typeof errData === 'string') {
                errorMessage = errData;
              } else if (errData.message) {
                errorMessage = errData.message;
              } else if (errData.errors) {
                errorMessage = Object.values(errData.errors).flat().join(', ');
              } else if (errData.title) {
                errorMessage = errData.title;
              }
            } catch {
              errorMessage = errorText;
            }
          }
          throw new Error(errorMessage);
        }

        setSuccess('Регистрация успешна! Теперь войдите.');
        setTimeout(() => setIsRegisterMode(false), 3000);
      } else {
        const response = await fetch(`${API_URL}/login`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            email: data.email,
            password: data.password
          })
        });

        if (!response.ok) {
          const errorText = await response.text();
          let errorMessage = 'Неверный email или пароль';

          if (errorText) {
            try {
              const errData = JSON.parse(errorText);
              errorMessage = errData.message || errorMessage;
            } catch {
              errorMessage = errorText;
            }
          }
          throw new Error(errorMessage);
        }

        const resData = await response.json();
        const token = resData.token;

        if (token) {
          // Обновляем состояние в AuthContext (сохранит токен и декодирует user)
          login(token);
          setSuccess('Успешный вход! Перенаправление...');

          setTimeout(() => {
            if (onClose) onClose();
            window.location.hash = '/dashboard';
          }, 600);
        } else {
          throw new Error('Сервер не вернул токен авторизации');
        }
      }
    } catch (err) {
      setError(err.message || 'Ошибка при обработке данных');
      console.error('Auth error:', err);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="auth-modal-overlay" onClick={onClose}>
      <div className="auth-modal" onClick={(e) => e.stopPropagation()}>
        <button 
          className="auth-modal-close" 
          onClick={onClose} 
          aria-label="Закрыть"
        >
          &times;
        </button>

        <div className="auth-form-container">
          <h2 className="auth-title">
            {isRegisterMode ? 'Регистрация' : 'Вход в аккаунт'}
          </h2>
          
          {success && <p className="auth-success" role="alert">{success}</p>}
          {error && <p className="auth-error" role="alert">{error}</p>}

          <form onSubmit={handleSubmit} className="auth-form">
            {isRegisterMode && (
              <div className="form-group">
                <label htmlFor="fullName" className="form-label">Имя и фамилия</label>
                <input 
                  type="text" 
                  id="fullName" 
                  name="fullName" 
                  className="form-input" 
                  placeholder="Иван Иванов" 
                  required 
                />
              </div>
            )}

            <div className="form-group">
              <label htmlFor="email-auth" className="form-label">Email</label>
              <input 
                type="email" 
                id="email-auth" 
                name="email" 
                className="form-input" 
                placeholder="example@mail.com" 
                required 
              />
            </div>

            <div className="form-group">
              <label htmlFor="password-auth" className="form-label">Пароль</label>
              <input 
                type="password" 
                id="password-auth" 
                name="password" 
                className="form-input" 
                placeholder={isRegisterMode ? "Минимум 8 символов" : "Введите пароль"} 
                minLength={isRegisterMode ? 8 : undefined}
                required 
              />
            </div>

            <button type="submit" className="auth-submit-btn" disabled={isLoading}>
              {isLoading ? 'Загрузка...' : (isRegisterMode ? 'Зарегистрироваться' : 'Войти')}
            </button>
          </form>

          <div className="auth-toggle">
            {isRegisterMode ? (
              <span>
                Уже есть аккаунт?{' '}
                <button type="button" className="toggle-btn" onClick={() => setIsRegisterMode(false)}>
                  Войти
                </button>
              </span>
            ) : (
              <span>
                Нет аккаунта?{' '}
                <button type="button" className="toggle-btn" onClick={() => setIsRegisterMode(true)}>
                  Зарегистрироваться
                </button>
              </span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default AuthModal;
