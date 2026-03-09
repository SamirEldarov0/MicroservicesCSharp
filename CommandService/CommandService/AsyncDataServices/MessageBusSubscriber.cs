
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IChannel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeRabbitMQAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(
                exchange: "trigger",
                type: ExchangeType.Fanout
            );
            var result = await _channel.QueueDeclareAsync();
            _queueName = result.QueueName;
            await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine(" --> Listening on the message bus...");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutDown;

        }
        public async ValueTask DisposeAsync()
        {
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }


        private async Task RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine(" --> Connection ShutDown...");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += (sender, ea) =>
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);

                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

            // Keep the service alive until shutdown
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}
