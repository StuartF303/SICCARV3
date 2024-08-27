using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
#nullable enable

namespace siccarcmd.User
{
    internal sealed class DeleteUserCommand : Command
    {
        private readonly UserServiceClient? _client;
        
        public DeleteUserCommand(string action, IServiceProvider services) : base(action)
        {
            Name = "delete";
            Description = "Delete a user";
            _client = services.GetService<UserServiceClient>();
            
            Add(new Argument<Guid>("id", "The user id"));
            Handler = CommandHandler.Create<Guid>(Delete);
        }

        private async Task Delete(Guid id)
        {
            if (_client != null)
            {
                var result = await _client.Delete(id);
                if (!result.IsSuccessStatusCode)
                    Console.WriteLine($"Delete user failed with status code {result.StatusCode}");
                else
                    Console.WriteLine("Deleted user");
            }
        }
    }
}
