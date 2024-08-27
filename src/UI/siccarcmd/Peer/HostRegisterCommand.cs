using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;

namespace siccarcmd.peer
{
    public class HostRegisterCommand : Command
    {
        public PeerServiceClient CommsHandler = null;

        public HostRegisterCommand(string action, IServiceProvider services) : base(action)
        {
            this.Name = "host";
            this.Description = "Ask the Peer to Transport a given Register";
            CommsHandler = services.GetService<PeerServiceClient>();
            Add(new Argument<string>("register", description: "A Register ID or Name"));
            Handler = CommandHandler.Create<string>(HostRegister);
        }

        private async void HostRegister(string register)
        {
            Console.WriteLine("Requesting Host Register ID : {0}", register);
            var result = await CommsHandler.HostRegister(register);
            Console.WriteLine("Returned : {0}", result);
        }
    }
}
