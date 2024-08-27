using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
#nullable enable

namespace siccarcmd.User
{
    internal sealed class UpdateRoleCommand : Command
    {
        private readonly UserServiceClient? _client;
        
        public UpdateRoleCommand(string action, IServiceProvider services) : base(action)
        {
            Name = "role";
            Description = "add or remove a user role with +/- role name (e.g +wallet.admin)";
            _client = services.GetService<UserServiceClient>();
            
            Add(new Argument<Guid>("id", "The user id"));
            Add(new Argument<string>("role", "The user role"));
            Handler = CommandHandler.Create<Guid, string>(UpdateRole);
        }

        private async Task UpdateRole(Guid id, string role)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                HttpResponseMessage? response = null;
                var addOrRemove = role.First().ToString();
                switch (addOrRemove)
                {
                    case "+":
                        response = await _client!.AddToRole(id, role.Replace("+", ""));
                        break;
                    case "-":
                        response = await _client!.RemoveFromRole(id, role.Replace("-", ""));
                        break;
                    default:
                        Console.WriteLine("Specify either + or - to add or remove a user role");
                        break;
                }

                if (response != null)
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
