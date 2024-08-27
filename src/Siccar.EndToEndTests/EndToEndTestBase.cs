using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Siccar.EndToEndTests
{
    [Collection("EndToEndTests")]
    public class EndToEndTestFixture : IDisposable
    {
        private static List<PubSubMessage> ReceivedMessages = new();
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private static int _retryCount;
        private readonly AsyncEventingBasicConsumer _consumer;
        
        public EndToEndTestFixture()
        {
#if DEBUG
            // Required for using the correct appsettings.json for EF Core
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif
            ReceivedMessages = new List<PubSubMessage>();

            var rabbitConnectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DispatchConsumersAsync = true
            };

            _connection = rabbitConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.Received += async (obj, args) =>
            {
                try
                {
                    var messageString = System.Text.Encoding.UTF8.GetString(args.Body.ToArray());
                    var messageData = JsonSerializer.Deserialize<PubSubMessage>(messageString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ReceivedMessages.Add(messageData!);
                    await Task.CompletedTask;
                }
                finally
                {
                    _channel.BasicAck(args.DeliveryTag, false);
                }
            };
        }

        public async Task WaitForPubSub(string exchange, string queue, Func<Task> task, params (string topic, int count)[] expectedMessagesAndCounts)
        {
            ConsumeQueue(exchange, queue);
            await task.Invoke();
            WaitForMessages(expectedMessagesAndCounts);
        }

        public async Task WaitForPubSub(string exchange, string queue, Func<Task> task, params (string topic, string transactionId, int count)[] expectedMessagesAndCounts)
        {
            ConsumeQueue(exchange, queue);
            await task.Invoke();
            WaitForTransactionMessages(expectedMessagesAndCounts);
        }

        public static void WaitForMessages(params (string topic, int count)[] expectedMessagesAndCounts)
        {
            int GetExpectedMessageCount(List<PubSubMessage> receivedMessages, (string topic, int count) expectedMessage)
            {
                return receivedMessages.Count(m => m.Topic == expectedMessage.topic);
            }

            var receivedMessages = ReceivedMessages.ToList();
            var failed = false;

            foreach (var expectedMessage in expectedMessagesAndCounts)
            {
                var actualMessageCount = GetExpectedMessageCount(receivedMessages, expectedMessage);
                if (actualMessageCount != expectedMessage.count)
                {
                    failed = true;
                }
            }

            // Retry for up to 10 seconds
            if (failed && _retryCount < 50)
            {
                _retryCount += 1;
                Thread.Sleep(200);
                WaitForMessages(expectedMessagesAndCounts);
                return;
            }

            foreach (var expectedMessage in expectedMessagesAndCounts)
            {
                Assert.Equal(expectedMessage.count, GetExpectedMessageCount(receivedMessages, expectedMessage));
            }
        }

        public static void WaitForTransactionMessages(params (string topic, string transactionId, int count)[] expectedMessagesAndCounts)
        {
            int GetExpectedMessageCount(List<PubSubMessage> receivedMessages, (string topic, string transactionId, int count) expectedMessage)
            {
                return receivedMessages.Count(m => m.Topic == expectedMessage.topic && m.Data?["transactionId"].ToString() == expectedMessage.transactionId);
            }

            var receivedMessages = ReceivedMessages.ToList();
            var failed = false;

            foreach (var expectedMessage in expectedMessagesAndCounts)
            {
                var actualMessageCount = GetExpectedMessageCount(receivedMessages, expectedMessage);
                if (actualMessageCount != expectedMessage.count)
                {
                    failed = true;
                }
            }

            // Tx messages can take a while, retry for up to 20 seconds
            if (failed && _retryCount < 100)
            {
                _retryCount += 1;
                Thread.Sleep(200);
                WaitForTransactionMessages(expectedMessagesAndCounts);
                return;
            }

            foreach (var expectedMessage in expectedMessagesAndCounts)
            {
                Assert.Equal(expectedMessage.count, GetExpectedMessageCount(receivedMessages, expectedMessage));
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _channel?.Dispose();
            _connection?.Dispose();
            ReceivedMessages = new();
            _retryCount = 0;
        }

        private void ConsumeQueue(string exchange, string queue)
        {
            _channel.ExchangeDeclare(exchange, "fanout", true);
            _channel.QueueDeclare(queue, true, autoDelete: true);
            _channel.QueueBind(queue, exchange, "#");
            _channel.BasicConsume(queue, false, _consumer);
        }
    }

    public class PubSubMessage
    {
        public string? Topic { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }
}
