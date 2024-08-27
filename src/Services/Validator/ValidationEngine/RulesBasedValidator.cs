// RulesBasedValidator Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Registers.ValidatorCore;
#nullable enable

namespace Siccar.Registers.ValidationEngine
{
    public class RulesBasedValidator : ISiccarValidator
    {
        public const string StoreName = "statestore";
        public const string PubSubName = "pubsub";
        private readonly IConfiguration Configuration;
        private readonly ILogger<RulesBasedValidator> Log;
        private readonly IDaprClientAdaptor Client;
        private readonly IMemPool MemPool;
        private readonly DocketBuilder Builder;
        private readonly Genesys GenesysBuilder;
        private readonly string DocketHash = "0000000000000000000000000000000000000000000000000000000000000000";
        private List<Register> Registers = new();
        private readonly IRegisterServiceClient RegisterServiceClient;
        UInt64 Height = 0;
        private readonly int delay = 10; // delay in seconds
        private Timer? validatorTimer = null;
        private CancellationToken token;
        private readonly JsonSerializerOptions jopts = new(JsonSerializerDefaults.Web);

        public RulesBasedValidator(IMemPool mempool, DocketBuilder builder, Genesys genesys, ILogger<RulesBasedValidator> logger, IDaprClientAdaptor client,
            IRegisterServiceClient registerServiceClient, IConfiguration configuration)
        {
            Configuration = configuration;
            MemPool = mempool;
            Log = logger;
            Client = client;
            Builder = builder;
            GenesysBuilder = genesys;
            RegisterServiceClient = registerServiceClient;
            string? delayStr = string.IsNullOrWhiteSpace(Configuration["Validator:CycleTime"]) ? "10" : Configuration["Validator:CycleTime"];
#pragma warning disable CS8604 // Possible null reference argument. Because no it wont
            delay = int.Parse(s: delayStr);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// IHostedService start method
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.LogInformation("[VALIDATOR] **** Started at: {dt} on a {td}s cycle", DateTimeOffset.Now, delay);
            token = cancellationToken;
            validatorTimer = new Timer(ProcessValidation, null, delay * 1000, delay * 1000);
            return Task.CompletedTask;
        }

        /// <summary>
        /// IHostedService stop method
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            token = cancellationToken;
            validatorTimer?.Dispose();
            Log.LogInformation("[VALIDATOR] **** Stopped at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the number of Registers being validated
        /// </summary>
        public uint RegistersCount { get { return (uint)Registers.Count; } }

        public JsonElement Status { get { return JsonSerializer.SerializeToElement(Registers, jopts); } }

        /// <summary>
        /// A single run through of the Validator Service 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async void ProcessValidation(object? state)
        {

            Registers = await RegisterServiceClient.GetRegisters();

            TimeSpan processTime = TimeSpan.Zero;

            if (Registers.Any())
            {
                try
                {
                    foreach (var procReg in Registers)
                    {
                        DateTime start = DateTime.UtcNow;
                        Log.LogDebug("Processing Register : {id}", procReg.Id);

                        if (procReg.Height < 1)
                        {
                            //todo: when we have network, ask the Network for the height first - this might be a recovery.
                            await GenesysBuilder.ProcessGenesys(procReg.Id);
                            Registers.Where(r => r.Id == procReg.Id).First().Height = 1;
                            break;
                        }

                        var previousDockets = await RegisterServiceClient.GetDocketByHeight(procReg.Id, procReg.Height - 1);
                        var localRegPool = MemPool.GetPool(procReg.Id);
                        Height = procReg.Height;

                        if (localRegPool.Any())
                        {
                            List<string> ids = new();
                            foreach (var entry in localRegPool)
                            {
                                ids.Add(entry.TxId);
                            }

                            var docket = Builder.GenerateDocket(new Docket()
                            {
                                Id = Height,
                                RegisterId = procReg.Id,
                                PreviousHash = DocketHash,
                                TransactionIds = ids,
                                TimeStamp = DateTime.UtcNow,
                                State = DocketState.Proposed
                            });
                            Log.LogInformation("[NEWDOCKET] Register: {rid}, Docket : {did}, Tx Count {tid}", docket.RegisterId, docket.Id,
                                docket.TransactionIds.Count);

                            // so we should push this out to the network and consensus but for the moment ram them home... 
                            await Client.PublishEventAsync<Docket>(PubSubName, Topics.DocketConfirmedTopicName, docket);
                            foreach (TransactionModel transaction in localRegPool)
                            {
                                await Client.PublishEventAsync<TransactionModel>(PubSubName, Topics.TransactionValidationCompletedTopicName, transaction);
                            }

                        }

                        processTime = DateTime.UtcNow.Subtract(start);
                        Log.LogInformation("Completed Register : {id} in {ms}ms", procReg.Id, processTime.Milliseconds);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        throw;
                    }
                    Log.LogError("Exception {msg} happend.", ex.Message);
                }
            }
            else { Log.LogWarning("No Registers Currently Configured"); }
        }
    }
}