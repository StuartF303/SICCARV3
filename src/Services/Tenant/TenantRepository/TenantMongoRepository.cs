using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Siccar.Platform.Tenants.Core;

namespace Siccar.Platform.Tenants.Repository
{
    /// <summary>
    /// Provides functionality  to persist "IdentityServer4.Models" into a given MongoDB
    /// </summary>
    public class TenantMongoRepository : ITenantRepository
    {
        private static IMongoClient? _client;
        private static IMongoDatabase? _database;


        /// <summary>
        /// This Contructor leverages  .NET Core built-in DI
        /// </summary>
        /// <param name="optionsAccessor">Injected by .NET Core built-in Depedency Injection</param>
        public TenantMongoRepository(IConfiguration Config)
        {
            _client = new MongoClient(Config["TenantRepository:MongoDBServer"]);
            _database = _client.GetDatabase(Config["TenantRepository:DatabaseName"]);
        }

        public async Task<IQueryable<T>> All<T>() where T : class, new()
        {
            return await Task.Run(() => _database?.GetCollection<T>(typeof(T).Name).AsQueryable()!);
        }

        public async Task<IQueryable<T>> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return await Task.Run(() => All<T>().Result.Where(expression));
        }

        public async Task<T> Single<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return await Task.Run(() => All<T>().Result.Where(expression).SingleOrDefault()!);
        }

        public async Task Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class, new()
        {
            await Task.Run(() => _database?.GetCollection<T>(typeof(T).Name).DeleteMany(predicate));
        }

        public async Task<bool> Exists<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _database?.GetCollection<T>(typeof(T).Name).Find(predicate).AnyAsync()!;
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = _database?.GetCollection<T>(typeof(T).Name);
            var filter = new BsonDocument();
            var totalCount = collection?.CountDocuments(filter);
            return (totalCount > 0);
        }

        public Task Add<T>(T item) where T : class, new()
        {
            _database?.GetCollection<T>(typeof(T).Name).InsertOne(item);
            return Task.CompletedTask;
        }

        public Task Add<T>(IEnumerable<T> items) where T : class, new()
        {
            _database?.GetCollection<T>(typeof(T).Name).InsertMany(items);
            return Task.CompletedTask;
        }

        public Task UpdateClient<T>(T item) where T : Client, new()
        {
            _database?.GetCollection<T>(typeof(T).Name).FindOneAndReplace(collectionItem => collectionItem.Id == item.Id, item);
            return Task.CompletedTask;
        }

        public Task UpdateTenant<T>(T item) where T : Tenant, new()
        {
            _database?.GetCollection<T>(typeof(T).Name).FindOneAndReplace(collectionItem => collectionItem.Id == item.Id, item);
            return Task.CompletedTask;
        }
    }
}