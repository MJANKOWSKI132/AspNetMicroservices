using System.Text;
using System.Text.Json;
using PlatformService.Dto;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private IConnection _connection;
        private IModel _channel;

        public MessageBusClient(IConfiguration config)
        {
            this._config = config;
            var factory = new ConnectionFactory
            {
                HostName = this._config["RabbitMQHost"],
                Port = int.Parse(this._config["RabbitMQPort"])
            };
            try
            {
                this._connection = factory.CreateConnection();
                this._channel = this._connection.CreateModel();
                this._channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                this._connection.ConnectionShutdown += this.RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);
            if (this._connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection open, sending message...");
                this.SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            this._channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
            );
            Console.WriteLine($"--> We have sent: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (this._channel.IsOpen)
            {
                this._channel.Close();
                this._connection.Close();
            }
        }
    }
}