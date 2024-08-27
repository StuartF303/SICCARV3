using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WalletService.IntegrationTests.Models;
using WebMotions.Fake.Authentication.JwtBearer;

namespace WalletService.IntegrationTests.Config
{
    [Collection("Wallet")]
    public class WalletTestBase : IDisposable
    {
        public static List<WalletMessage> ReceivedMessages = new();
        public readonly WalletOperations WalletOperations;

        private readonly WalletWebApplicationFactory<WebHostStartup> _factory;
        private readonly bool _isPubSubTest;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;
        private static int _retryCount;
        private const string WalletTestQueue = "WalletService.IntegrationTests";
        private const string WalletExchange = "OnWallet_AddressCreated";

        public WalletTestBase(WalletWebApplicationFactory<WebHostStartup> factory, bool isPubSubTest, bool bypassAuthentication)
        {
            _factory = factory;
            _isPubSubTest = isPubSubTest;
            WalletOperations = new(CreateClient(bypassAuthentication));

            if (_isPubSubTest)
            {
                ReceivedMessages = new List<WalletMessage>();
                
                var rabbitConnectionFactory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                    DispatchConsumersAsync = true
                };

                _connection = rabbitConnectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (obj, args) =>
                {
                    try
                    {
                        var messageString = System.Text.Encoding.UTF8.GetString(args.Body.ToArray());
                        var messageData = JsonSerializer.Deserialize<WalletMessage>(messageString,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        ReceivedMessages.Add(messageData!);
                        await Task.CompletedTask;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        _channel.BasicAck(args.DeliveryTag, false);
                    }
                };
                _channel.ExchangeDeclare(WalletExchange, "fanout", true);
                _channel.QueueDeclare(WalletTestQueue, true, autoDelete: true);
                _channel.QueueBind(WalletTestQueue, WalletExchange, "#");
                _channel.BasicConsume(WalletTestQueue, false, consumer);
            }
        }

        private HttpClient CreateClient(bool bypassAuthorisation)
        {
            var path = Assembly.GetAssembly(typeof(WebHostStartup))!.Location;

            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // Fake auth
                        services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
                    });
                    builder
                        .UseStartup(context => new WebHostStartup(context.Configuration, bypassAuthorisation));
                    builder.UseContentRoot(Path.GetDirectoryName(path)!);
                    builder.ConfigureAppConfiguration(cb =>
                    {
                        cb.AddJsonFile("appsettings.Development.json", optional: false)
                            .AddEnvironmentVariables();
                    });
                })
                .CreateClient();

            return client;
        }

        public static void PubSubMessagesWereReceived(
            params (string topic, int count, string walletId)[] expectedMessagesAndCounts)
        {
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

            // Retry for up to 2 seconds
            if (failed && _retryCount < 10)
            {
                _retryCount += 1;
                Thread.Sleep(200);
                PubSubMessagesWereReceived(expectedMessagesAndCounts);
                return;
            }

            foreach (var expectedMessage in expectedMessagesAndCounts)
            {
                Assert.Equal(expectedMessage.count, GetExpectedMessageCount(receivedMessages, expectedMessage));
            }
        }

        private static int GetExpectedMessageCount(List<WalletMessage> receivedMessages, (string topic, int count, string walletId) expectedMessage)
        {
            return receivedMessages.Count(m => m.Topic == expectedMessage.topic && m?.Data?.WalletId == expectedMessage.walletId);
        }

        public void Dispose()
        {
            if (_isPubSubTest)
            {
                _factory.Dispose();
                _channel?.Dispose();
                _connection?.Dispose();
                ReceivedMessages = new();
                _retryCount = 0;
            }
        }
    }
}
