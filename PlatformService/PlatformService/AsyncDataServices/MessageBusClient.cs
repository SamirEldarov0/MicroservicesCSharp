using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IChannel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeRabbitMq().GetAwaiter().GetResult();
        }

        // 🔒 Interface requires SYNC method
        public async Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto)
        {

            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (_channel.IsOpen)
            {
                Console.WriteLine(" --> RabbitMQ connection open, sending message...");
                await SendMessageAsync(message);
            }
            else
            {
                Console.WriteLine("RabbitMQ connection closed, ot sending message");
            }
        }

        public async Task Dispose()
        {
            Console.WriteLine("Message Bus Disposed");
            if (_channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _connection.CloseAsync();
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: "trigger",
                routingKey: "",
                //basicProperties: null,
                body: body
            );
            Console.WriteLine($"We have sent message : {message}");
            //await _channel.BasicPublishAsync(
            //    exchange: "trigger",
            //    routingKey: string.Empty,
            //    basicProperties: null,
            //    body: body
            //);

        }
        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> Connection Shutdown");
        }

        private async Task InitializeRabbitMq()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
                //UserName = _configuration["RabbitMQUser"],
                //Password = _configuration["RabbitMQPassword"]
            };

            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(
                    exchange: "trigger",
                    type: ExchangeType.Fanout
                );
                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("✅ Connected to RabbitMQ Message Bus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Couldn't connect to the message bus - {ex.Message}");
            }
        }
    }
}