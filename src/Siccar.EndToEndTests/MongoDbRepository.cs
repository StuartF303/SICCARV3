using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Siccar.Platform;
using static System.String;

namespace Siccar.EndToEndTests
{
    internal class MongoDbRepository
    {
        private readonly string _connectionString = "mongodb://127.0.0.1:27017";
        private readonly MongoClient _mongoClient;

        public MongoDbRepository()
        {
           var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.migration.json")
                .AddEnvironmentVariables()
                .Build();

            _connectionString = IsNullOrEmpty(configuration["RegisterRepository:MongoDBServer"]) 
                ? _connectionString : configuration["RegisterRepository:MongoDBServer"]!;
            _mongoClient = new MongoClient(_connectionString);
        }

        public async Task Clear()
        {
            await _mongoClient.DropDatabaseAsync("RegisterService");
        }

        public async Task<int> GetTransactionCount(string registerId, string transactionId)
        {
            var db = _mongoClient.GetDatabase("RegisterService");
            var collection = db.GetCollection<TransactionModel>(registerId);
            return (int)await collection.CountDocumentsAsync(t => t.Id == transactionId);
        }
    }
}
