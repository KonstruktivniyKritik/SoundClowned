import React from 'react';
import './LandingPage.css';

function LandingPage() {
    // В реальном приложении здесь была бы логика навигации или вызова модального окна
    const handleAuthClick = () => {
        // Заменил alert на console.log, чтобы избежать проблем в iframe
        console.log('Переход к окну авторизации...');
    };

    return (
        <div className="landing-container">
            <header className="header">
                {/* ДОБАВЛЕНА ОБЕРТКА ДЛЯ ЦЕНТРИРОВАНИЯ */}
                <div className="header-content"> 
                    <h1 className="logo">SOUNDCLOWNED</h1>
                </div>
            </header>
            
            <main className="hero-section">
                {/* Содержимое hero-section центрируется автоматически через align-items: center в CSS */}
                <h2 className="tagline">
                    Откройте для себя больше музыки. Слушайте, <br/> 
                    загружайте и делитесь с миллионами.
                </h2>
                
                <p className="description">
                    Присоединяйтесь к крупнейшему в мире сообществу артистов, фанатов и аудиофилов. 
                    Начните слушать тысячи часов бесплатной музыки прямо сейчас.
                </p>

                <button 
                    className="auth-button"
                    onClick={handleAuthClick}
                >
                    Войти на сайт
                </button>
            </main>
            
            <footer className="footer">
                <p>&copy; 2024 SOUNDCLOWNED. Все права на музыку принадлежат авторам.</p>
            </footer>
        </div>
    );
}

export default LandingPage;
