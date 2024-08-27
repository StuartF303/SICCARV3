using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;

namespace siccarcmd.register
{
    /// <summary>
    /// A set of commands that display the status and details of a given register
    /// can be put in a real-time mode that pumps updates as they happen
    /// </summary>
    public class InfoCommand : Command
    {
        public IRegisterServiceClient commshandler = null;

        public InfoCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "info";
            this.Description = "Displays information about the Register Service";

            commshandler = services.GetService<IRegisterServiceClient>();

            Add(new Argument<string>("register", () => "", description: "Get deails on named register, if not specified all on the peer."));
            Add(new Option<bool>(new string[] { "-u", "--update" }, () => false, description: "Display a continuous monitor feed.(NotYet)"));
            Handler = CommandHandler.Create<string>(GetInfo);
        }

        private async Task GetInfo(string register)
        {
            if (String.IsNullOrEmpty(register))
            { // display all registers

                var regs = await commshandler.GetRegisters();

                if (regs.Count > 0)
                {
                    Console.WriteLine(" Registers ({0}): [id], [name] ", regs.Count);
                    foreach (var r in regs)
                    {
                        Console.WriteLine(" \t\t {0} \t {1}", r.Id, r.Name);
                    }
                }
                else
                    Console.WriteLine("No Registers found.");
            }
            else
            { // display details on named only
                var regState = await commshandler.GetRegister(register);
                Console.WriteLine("Register State : \n {0}", regState.Name);
            }
        }
    }
}
