using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Siccar.Common.Adaptors;
using Siccar.Common.ServiceClients;
using Siccar.Registers.ValidatorCore;
using System;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

namespace Siccar.Registers.ValidationEngine
{
    /// <summary>
    /// A simple singular Consent Model that take a proposed Docket and Commits it to the Register
    /// </summary>
    public class SingularConsensus : ISiccarConsensus
    {
        public uint RegistersCount { get; set; }

        public bool NetworkMaster => throw new NotImplementedException();

        private Timer? validatorTimer = null;
        int delay = 10;
        CancellationToken token;
        private readonly IConfiguration Configuration;
        private readonly ILogger<SingularConsensus> Log;
        private readonly IDaprClientAdaptor Client;
        private readonly IMemPool MemPool;
        private readonly IRegisterServiceClient RegisterServiceClient;


        public SingularConsensus(IMemPool mempool, ILogger<SingularConsensus> logger, IDaprClientAdaptor client,
            IRegisterServiceClient registerServiceClient, IConfiguration configuration)
        {
            Configuration = configuration;
            MemPool = mempool;
            Log = logger;
            Client = client;
            RegisterServiceClient = registerServiceClient;
            string? time = Configuration["Validator:CycleTime"];
            delay = string.IsNullOrWhiteSpace(time) ? 10 : int.Parse(time);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.LogInformation("[CONSENSUS] **** Started at: {dto} on a {cycle}s cycle", DateTimeOffset.Now, delay);
            token = cancellationToken;
            validatorTimer = new Timer(ProcessConsent, null, (delay + (delay/2)) * 1000, delay * 1000);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            token = cancellationToken;
            validatorTimer?.Dispose();
            Log.LogInformation("[CONSENSUS] **** Stopped at: {time}", DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        /// <summary>
        /// We want to make Proposed Dockets commited
        /// </summary>
        /// <param name="state"></param>
        private async void ProcessConsent(object? state)
        {
            await Task.Run(() => {});
        }

    }
}
