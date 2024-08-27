using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.Platform;

namespace siccarcmd.register
{
    /// <summary>
    /// Register Create Command
    /// </summary>
    public class CreateCommand : Command
    {
        public IRegisterServiceClient commshandler = null;

        /// <summary>
        /// Create Register Command Interface Parser
        /// </summary>
        /// <param name="action"></param>
        public CreateCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "create";
            this.Description = "Creates a new Registers";

            Add(new Argument<string>("name", description: "A friendly name for the Register."));
            // removed Add(new Option<string>(new string[] { "-i", "--id" }, description: "A specific Id for the Register, otherwise auto generated."));
            Add(new Option<bool>(new string[] { "-a", "--advertise" }, () => false, description: "Will set the Register to broadcast its presence."));
            Add(new Option<bool>(new string[] { "-f", "--full" }, () => true, description: "Will set the local service to only store all transactions."));

            //commshandler = services.GetRequiredService<IRegisterServiceClient>();
            commshandler = services.GetService<IRegisterServiceClient>();
            Handler = CommandHandler.Create<string, string, bool, bool>(CreateRegister);
        }

        /// <summary>
        /// Register Create Command Handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private async Task CreateRegister(string name, string id = null, bool advertise = true, bool full = true)
        {
            var newReg = new Siccar.Platform.Register()
            {
                // is created and returned from the server Id = id,
                Name = name,
                Advertise = advertise,
                IsFullReplica = full,
                Status = RegisterStatusTypes.ONLINE
            };

            var ret = await commshandler.CreateRegister(newReg);
            Console.WriteLine("Created Register : {0} [{1}]", newReg.Name, ret);
        }
    }
}
