using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
#nullable enable

namespace siccarcmd.User
{
    internal sealed class GetUserCommand : Command
    {
        private readonly UserServiceClient? _client;
        
        public GetUserCommand(string action, IServiceProvider services) : base(action)
        {
            Name = "get";
            Description = "Get a user";
            _client = services.GetService<UserServiceClient>();
            Add(new Argument<Guid>("id", "The user id"));
            Handler = CommandHandler.Create<Guid>(Get);
        }

        private async Task Get(Guid id)
        {
            if (_client != null)
            {
                var result = await _client.Get(id);
                Console.WriteLine(result.RootElement.GetRawText());
            }
        }
    }
}
