using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Application;
#nullable enable

namespace siccarcmd.Tenant
{
    internal sealed class PublishParticipantCommand : Command
    {
        private readonly ITenantServiceClient _client;

        public PublishParticipantCommand(string action, IServiceProvider services) : base(action)
        {
            _client = services.GetService<ITenantServiceClient>()!;
            Name = "publish";
            Description = "Publish a participant";
            Add(new Argument<string>("registerId", "The register id"));
            Add(new Argument<string>("walletAddress", "The wallet address"));
            Add(new Argument<string>("participantName", "The participant name"));
            Add(new Argument<string>("organisationName", "The organisation name"));
            Add(new Argument<string?>("didUri", () => null, "The published identity uri"));
            Add(new Argument<bool>("useStealthAddress", () => false, "Use hidden/anonymised wallet address"));
            Handler = CommandHandler.Create<string, string, string, string, string?, bool>(PublishParticipant);
        }

        private async Task PublishParticipant(string registerId, string walletAddress, string participantName, string organisationName, string? didUri, bool useStealthAddress)
        {
            var participant = new Participant
            {
                Name = participantName,
                Organisation = organisationName,
                WalletAddress = walletAddress,
                didUri = didUri,
                useStealthAddress = useStealthAddress
            };
            await _client.PublishParticipant(registerId, walletAddress, participant);
            Console.WriteLine($"Participant '{participantName}' published successfully");
        }
    }
}
