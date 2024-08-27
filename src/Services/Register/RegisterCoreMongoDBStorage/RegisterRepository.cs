using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Siccar.Platform;
using Siccar.Registers.Core.Storage;

namespace Siccar.Registers.Core.MongoDBStorage
{
    public class RegisterRepository : IRegisterRepository
    {
        /// Privates
        private const string RegisterStateDB = "RegisterService";
        private const string RegisterStateCollection = "LocalRegisters";
        private readonly IMongoClient _mongoClient = null;
        private readonly MongoDatabaseSettings dbSettings = null;
        private readonly IConfiguration Configuration = null;
        private static string defaultServer = "mongodb://127.0.0.1:27017";
        private MongoDatabaseBase _metadb = null;
        private IMongoCollection<Register> _regCollection = null;
        private readonly List<string> localRegisters = new();
        private ILogger _logger;


        /// <summary>
        /// This is the base MongoDB Storage layer for the Register Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public RegisterRepository(ILogger<RegisterRepository> logger, IConfiguration configuration)
        {
            //setup
            this.Configuration = configuration;
            this._logger = logger;

            dbSettings = new MongoDatabaseSettings();

            defaultServer = String.IsNullOrEmpty(Configuration["RegisterRepository:MongoDBServer"]) ? defaultServer : Configuration["RegisterRepository:MongoDBServer"];

            var mongoConnectionUrl = new MongoUrl(defaultServer);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
            mongoClientSettings.ClusterConfigurator = cb => {
                cb.Subscribe<CommandStartedEvent>(e => {
                    if(logger.IsEnabled(LogLevel.Debug))
                    {
                        logger.Log(LogLevel.Debug, $"Executing MongoDB Command: {e.Command.ToJson()} ");
                    }
                });
            };

            _mongoClient = new MongoClient(mongoClientSettings);
            logger.LogDebug("Repository Initialised on Database Server {server}", defaultServer);

            _metadb = (MongoDatabaseBase)_mongoClient.GetDatabase(RegisterStateDB, dbSettings);

            logger.LogDebug("Register Collections on {state}", RegisterStateDB);
            try
            {
                _regCollection = _metadb.GetCollection<Register>(RegisterStateCollection);
                localRegisters = _regCollection.Find<Register>(_ => true).ToEnumerable().Select(r => r.Id).ToList<string>();
                logger.LogInformation($"Local Register Collection loaded {_regCollection.EstimatedDocumentCount()} ", RegisterStateCollection);
            }
            catch (MongoClientException mongoError)
            {
                logger.LogError("MongoDB Client Error : {msg}", mongoError.Message);
                throw new RegisterRespositoryException(mongoError.Message);
            }

        }

        /// <summary>
        /// We are going to use a cache in a list of local registers to make this faster
        /// </summary>
        /// <param name="RegisterId"></param>
        /// <returns></returns>
        public async Task<bool> IsLocalRegisterAsync(string RegisterId)
        {
            return await Task.Run(() => localRegisters.Contains(RegisterId));
        }


        /// <summary>
        /// List the Registers on the Connected Server
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Register>> GetRegistersAsync()
        {
            // so we need to get the list from the Meta Store

            return await Task.Run(() => _regCollection.Find<Register>(_ => true).ToEnumerable<Register>());

        }

        public async Task<IEnumerable<Register>> QueryRegisters(Func<Register, bool> predicate)
        {
            var registers = _regCollection.Find(_ => true).ToList();
            return await Task.FromResult(registers.Where(predicate));
        }

        public async Task<Register> GetRegisterAsync(string registerId)
        {
            if (!await IsLocalRegisterAsync(registerId))
                throw new RegisterDoesNotExistException(registerId);

            return _regCollection.Find<Register>(r => r.Id == registerId).FirstOrDefault();
        }

        public async Task<Register> UpdateRegisterAsync(Register register)
        {
            if (!await IsLocalRegisterAsync(register.Id))
                throw new RegisterDoesNotExistException(register.Id);

            _regCollection.FindOneAndReplace(r => r.Id == register.Id, register);

            return register;
        }

        public async Task<Register> InsertRegisterAsync(Register newRegister)
        {
            // create a new 
            _metadb.GetCollection<Register>(newRegister.Id);

            await _regCollection.InsertOneAsync(newRegister);
            localRegisters.Add(newRegister.Id);

            return newRegister;
        }


        public async Task DeleteRegisterAsync(string registerId)
        {
            if (!(await IsLocalRegisterAsync(registerId)))
                throw new RegisterDoesNotExistException(registerId);

            localRegisters.Remove(registerId);
            _regCollection.FindOneAndDelete<Register>(r => r.Id == registerId);
            await _metadb.DropCollectionAsync(registerId);

        }

        public async Task<IEnumerable<Docket>> GetDocketsAsync(string RegisterId)
        {
            if (!(await IsLocalRegisterAsync(RegisterId)))
                throw new RegisterDoesNotExistException(RegisterId);

            IMongoCollection<Docket> str = _metadb.GetCollection<Docket>(RegisterId);
            return str.Find<Docket>(t => t.MetaData.TransactionType == TransactionTypes.Docket).ToEnumerable<Docket>();   //_ => true
        }

        public async Task<Docket> GetDocketAsync(string RegisterId, ulong DocketId)
        {
            if (!(await IsLocalRegisterAsync(RegisterId)))
                throw new RegisterDoesNotExistException(RegisterId);

            IMongoCollection<Docket> str = _metadb.GetCollection<Docket>(RegisterId);
            return str.Find<Docket>(d => d.Id == DocketId).FirstOrDefault();
        }

        public async Task<Docket> InsertDocketAsync(Docket docket)
        {
            if (!(await IsLocalRegisterAsync(docket.RegisterId)))
                throw new RegisterDoesNotExistException(docket.RegisterId);

            IMongoCollection<Docket> str = _metadb.GetCollection<Docket>(docket.RegisterId);
            try
            {
                await str.InsertOneAsync(docket);
            }
            catch (MongoBulkWriteException ex) 
            {
                if(!ex.ErrorLabels.Contains("DuplicateKey"))
                    _logger.LogError(ex.Message);
            }
            return docket;

        }

        public async Task<IQueryable<TransactionModel>> GetTransactionsAsync(string RegisterId)
        {
            if (!(await IsLocalRegisterAsync(RegisterId)))
                throw new RegisterDoesNotExistException(RegisterId);

            var transactionCollection = await EnsureCompoundIndex(RegisterId);
            var result = transactionCollection.AsQueryable().Where(t => t.MetaData.TransactionType != TransactionTypes.Docket);
            return result;
        }

        public async Task<TransactionModel> GetTransactionAsync(string RegisterId, string TransactionId)
        {
            if (!(await IsLocalRegisterAsync(RegisterId)))
                throw new RegisterDoesNotExistException(RegisterId);

            IMongoCollection<TransactionModel> str = _metadb.GetCollection<TransactionModel>(RegisterId);
            var tx = str.Find<TransactionModel>(t => t.TxId == TransactionId).FirstOrDefault();

            return tx;
        }

        public async Task<TransactionModel> InsertTransactionAsync(TransactionModel transaction)
        {
            if (!(await IsLocalRegisterAsync(transaction.MetaData.RegisterId)))
                throw new RegisterDoesNotExistException(transaction.MetaData.RegisterId);

            IMongoCollection<TransactionModel> str = _metadb.GetCollection<TransactionModel>(transaction.MetaData.RegisterId);
            await str.InsertOneAsync(transaction);

            return transaction;
        }

        public async Task<IEnumerable<TransactionModel>> QueryTransactions(string RegisterId, Expression<Func<TransactionModel, bool>> predicate)
        {
            if (!(await IsLocalRegisterAsync(RegisterId)))
                throw new RegisterDoesNotExistException(RegisterId);

            var transactionCollection = await EnsureCompoundIndex(RegisterId);
            return transactionCollection.AsQueryable<TransactionModel>().Where(predicate);
        }

        public async Task<IEnumerable<TransactionModel>> QueryTransactionPayload(string RegisterId, Expression<Func<TransactionModel, bool>> predicate)
        {
            if (!(await IsLocalRegisterAsync(RegisterId)))
                throw new RegisterDoesNotExistException(RegisterId);

            // we are going to search payloads for given parameters 
            throw new NotImplementedException();
        }

        public async Task<int> CountRegisters()
        {
            return (int)await _regCollection.CountDocumentsAsync(new BsonDocument());
        }

        private async Task<IMongoCollection<TransactionModel>> EnsureCompoundIndex(string registerId)
        {
            var transactionModelIndexKeys = Builders<TransactionModel>.IndexKeys;
            var indexModel = new CreateIndexModel<TransactionModel>(
                transactionModelIndexKeys.Descending(x => x.TimeStamp).Ascending(x => x.TxId), new CreateIndexOptions
                {
                    Name = "Index_TimeStamp_TxId"
                });

            var transactionCollection = _metadb.GetCollection<TransactionModel>(registerId);
            await transactionCollection.Indexes.CreateOneAsync(indexModel);
            return transactionCollection;
        }
    }
}
