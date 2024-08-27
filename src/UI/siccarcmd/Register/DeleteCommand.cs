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
    public class DeleteCommand : Command
    {
        public IRegisterServiceClient commshandler = null;

        /// <summary>
        /// Create Register Command Interface Parser
        /// </summary>
        /// <param name="action"></param>
        public DeleteCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "delete";
            this.Description = "Deletes a Registers from an installation";

            Add(new Argument<string>("id", description: "The Register ID"));

            //commshandler = services.GetRequiredService<IRegisterServiceClient>();
            commshandler = services.GetService<IRegisterServiceClient>();
            Handler = CommandHandler.Create<string>(DeleteRegister);
        }

        /// <summary>
        /// Register Create Command Handler
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        private async Task DeleteRegister(string id)
        {

            Console.WriteLine($"!! This action will delete and remove storage of the register with the given id: {id}");

            Register delReg = await commshandler.GetRegister(id);
            Console.WriteLine($" Are you sure, delete the register {delReg.Name}");
            Console.WriteLine("Press 'Y' to delete ");
            var key = Console.ReadKey();

            if ((key.KeyChar == 'y') || (key.KeyChar == 'Y'))
            {
                try
                {
                    var ret = await commshandler.DeleteRegister(id);
                    Console.WriteLine($"Delete Register : {id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete Register : {ex.Message}");
                }
            }
        }
    }
}
