# Система Авторизации SoundClowned

## Архитектура

Система авторизации состоит из трех компонентов:

### 1. Backend (Identity Provider)
- **Порт:** 5002
- **Эндпоинты:**
  - `POST /api/auth/login` — вход пользователя
  - `POST /api/auth/register` — регистрация нового пользователя

**Отправка данных для входа:**
```json
{
  "username": "email@example.com",
  "password": "your_password"
}
```

**Ответ от сервера:**
```json
{
  "message": "Login Succesfully",
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

### 2. Backend (Api Gateway)
- **Порт:** 5001
- Функция: маршрутизация запросов, проверка JWT-токенов
- Настроен CORS для работы с фронтендом

### 3. Frontend (React SPA)
- **Порт:** 3000 (localhost:3000)
- Компоненты:
  - [AuthModal.jsx](/Users/spiryukov.artem/Documents/SoundClowned/SoundClowned/client/src/AuthModal.jsx) — модальное окно авторизации
  - [App.jsx](/Users/spiryukov.artem/Documents/SoundClowned/SoundClowned/client/src/App.jsx) — основной компонент с AuthProvider
  - [AuthContext.jsx](/Users/spiryukov.artem/Documents/SoundClowned/SoundClowned/client/src/context/AuthContext.jsx) — управление состоянием авторизации

## Как это работает

1. Пользователь нажимает "Войти на сайт" → открывается модальное окно
2. Ввод email и пароля → отправка запроса на `/api/auth/login`
3. Сервер возвращает JWT-токен → фронтенд сохраняет в `localStorage`
4. При загрузке страницы проверяется наличие токена
5. При наличии токена показывается кнопка "Выйти"

## Токены безопасности

- JWT-токен сохраняется в `localStorage` с именем `token`
- При декодировании токена извлекаются данные пользователя (email, username)
- При выходе (`logout`) токен удаляется

## Настройка Docker

Убедитесь, что в `docker-compose.yml` настроены переменные:
```yaml
environment:
  - JwtSettings:Secret=your_jwt_secret_here
  - JwtSettings:Issuer=http://identity-provider
  - JwtSettings:Audience=soundclowned
```

## Файлы проекта

### Frontend
- `/client/src/AuthModal.jsx` — модальное окно входа/регистрации
- `/client/src/App.jsx` — основной компонент приложения
- `/client/src/context/AuthContext.jsx` — контекст авторизации
- `/client/src/LandingPage.jsx` — главная страница с кнопкой входа

### Backend
- `/server/Identity Provider/` — сервис выдачи JWT-токенов
- `/server/ApiGateway/` — шлюз для маршрутизации запросов
