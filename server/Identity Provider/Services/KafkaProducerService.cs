using System.Text.Json;
using Confluent.Kafka;
using IdentityProvider.Events;

namespace IdentityProvider.Services
{
    /// <summary>
    /// Реализация IEventProducer для отправки сообщений в Kafka.
    /// </summary>
    public class KafkaProducerService : IEventProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        // Имя Kafka-сервиса в Docker Compose
        public KafkaProducerService(ILogger<KafkaProducerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            
            // Читаем адрес брокера из конфигурации: KafkaSettings__BrokerList
            // Используется ключ с двойным подчеркиванием, соответствующий docker-compose
            var kafkaBroker = configuration["KafkaSettings:BrokerList"]; 

            if (string.IsNullOrEmpty(kafkaBroker))
            {
                 // На случай, если переменная не была установлена (не должно произойти при правильном docker-compose)
                _logger.LogError("Kafka broker address not found in configuration (KafkaSettings:BrokerList). FALLING BACK to default 'kafka:9092'.");
                throw new Exception("Kafka adress in not set");
            }
            
            // Конфигурация продюсера
            var config = new ProducerConfig 
            { 
                BootstrapServers = kafkaBroker 
            };

            // Создаем экземпляр продюсера
            _producer = new ProducerBuilder<string, string>(config).SetKeySerializer(Serializers.Utf8).Build();
            _logger.LogInformation("Kafka Producer initialized for broker: {Broker}", kafkaBroker);
        }


        /// <summary>
        /// Отправляет сообщение в топик.
        /// </summary>
  public async Task ProduceAsync<TEvent>(string topic, string? key, TEvent message) where TEvent : class
        {
            if (message == null) return;

            // Сериализуем объект TEvent в JSON-строку
            var messageString = JsonSerializer.Serialize(message); 
            
            try
            {
                var deliveryReport = await _producer.ProduceAsync(
                    topic, 
                    new Message<string, string> { Key = key, Value = messageString }
                );

                _logger.LogInformation(
                    "Сообщение успешно отправлено в топик '{Topic}' с ключом '{Key}'. Offset: {Offset}",
                    topic, key, deliveryReport.Offset);
            }
            catch (ProduceException<string, string> e)
            {
                _logger.LogError(e, "Ошибка отправки сообщения в Kafka: {Error}", e.Error.Reason);
            }
        }
    }
}
