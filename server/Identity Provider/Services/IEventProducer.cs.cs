using IdentityProvider.Events;

namespace IdentityProvider.Services
{
    public interface IEventProducer
    {
        /// <summary>
        /// Отправка сообщения брокеру
        /// </summary>
        /// <param name="topic">топик брокера</param>
        /// <param name="key">ключ</param>
        /// <param name="SendMessage">сообщение</param>
        /// <returns></returns>
        Task ProduceAsync<TEvent>(string topic, string key, TEvent message) where TEvent : class;
    }
}