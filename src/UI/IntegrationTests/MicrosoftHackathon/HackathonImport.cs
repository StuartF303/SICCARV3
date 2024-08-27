using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using Siccar.Application;
using Siccar.Platform;
using System.Text.Json;
using Json.More;
using Siccar.Common.ServiceClients;

namespace IntegrationTests.MicrosoftHackathon
{
    public class HackathonImport : IDisposable
    {
        readonly IBlueprintServiceClient _blueprintServiceClient;
        readonly WalletServiceClient _walletServiceClient;
        readonly RegisterServiceClient _registerServiceClient;
        readonly ActionServiceClient _actionServiceClient;

        private Register _register;

        private string blueprintId = string.Empty;
        private List<Wallet> _wallets = new List<Wallet>();
        private IEnumerable<JsonElement> blueprintRuns;
        private int _runIndex = 0;
        private Dictionary<string, Dictionary<string, object>>? currentRunData;
        private Blueprint? blueprint;

        public HackathonImport(IServiceProvider serviceProvider, string bearer)
        {
            _actionServiceClient = (ActionServiceClient)serviceProvider.GetService<IActionServiceClient>()!;
            if (_actionServiceClient == null)
                throw new Exception("Cannot instantiate service client [ActionServiceClient]");
            _blueprintServiceClient = serviceProvider.GetService<IBlueprintServiceClient>()!;
            if (_blueprintServiceClient == null)
                throw new Exception("Cannot instantiate service client [BlueprintServiceClient]");
            _walletServiceClient = (WalletServiceClient)serviceProvider.GetService<IWalletServiceClient>()!;
            if (_walletServiceClient == null)
                throw new Exception("Cannot instantiate service client [WalletServiceClient]");
            _registerServiceClient = (RegisterServiceClient)serviceProvider.GetService<IRegisterServiceClient>()!;
            if (_registerServiceClient == null)
                throw new Exception("Cannot instantiate service client [RegisterServiceClient]");
        }

        public async Task<string> SetupTest()
        {
            // Create a register
            var registerName = "SiccarVysusHack";
            var currentRegister = (await _registerServiceClient.GetRegisters()).FirstOrDefault(r => r.Name == registerName);

            if (currentRegister == null)
            {
                var register = await _registerServiceClient.CreateRegister(
                    new Register
                    {
                        Advertise = false,
                        Name = registerName
                    });

                Console.WriteLine($"Created new Register : {register.Id}");
                _register = register;
            }
            else
            {
                _register = currentRegister;
                Console.WriteLine($"Loaded existing register : {_register.Id}");
            }

            var hackathonJson = await File.ReadAllTextAsync("MicrosoftHackathon/hackathon.json");
            var importData = JsonDocument.Parse(hackathonJson);
            var wallets = importData.RootElement.GetProperty("wallets").EnumerateArray();
            blueprintRuns = importData.RootElement.GetProperty("blueprintRuns").EnumerateArray().Select(run => run);

            // Create the wallets
            foreach (var wallet in wallets)
            {
                var existingWallet = await _walletServiceClient.GetWallet(wallet.GetProperty("address").GetString()!);

                if (existingWallet == null)
                {
                    var walletName = wallet.GetProperty("name").GetString()!;
                    var walletMnemonic = wallet.GetProperty("mnemonic").GetString()!;
                    existingWallet = await _walletServiceClient.CreateWallet(walletName, "", walletMnemonic);
                    Console.WriteLine($"Created wallet {wallet.GetProperty("name").GetString()} - {wallet.GetProperty("address").GetString()}");
                }
                else
                {
                    Console.WriteLine($"Loaded existing wallet {existingWallet.Name} - {existingWallet.Address}");
                }
                _wallets.Add(existingWallet);
            }

            JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

            // Create the blueprint
            blueprint = JsonSerializer.Deserialize<Blueprint>(importData.RootElement.GetProperty("blueprint"), serializerOptions);
            var blueprintTransactionId = await _blueprintServiceClient.PublishBlueprint("ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj", _register.Id, blueprint);

            Console.WriteLine($"Blueprint created and published, transaction id: {blueprintTransactionId.Id}");
            Console.WriteLine($"Waiting for blueprint transaction...");

            blueprintId = blueprintTransactionId.Id;
            return blueprintTransactionId.Id;
        }

        public async Task Import()
        {
            Siccar.Application.Action? startAction = null;
            Console.Write("Waiting for blueprint transaction confirmation");

            while (startAction == null)
            {
                Console.Write(".");
                try
                {
                    startAction = await _actionServiceClient.GetAction(_wallets.First(w => w.Name == "Anchor Handling Vessel").Address!, _register.Id, blueprintId);
                }
                catch (Exception er)
                {
                    // Ignore whilst we wait 
                }
                Thread.Sleep(500);
            }

            Console.WriteLine("Blueprint transaction confirmed");

            var currentRun = blueprintRuns.ElementAt(_runIndex);
            currentRunData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(currentRun);
            var currentRunActionData = currentRunData[startAction.Title];

            var actionSubmit = new ActionSubmission
            {
                BlueprintId = blueprintId,
                RegisterId = _register.Id,
                WalletAddress = _wallets.First(w => w.Name == "Anchor Handling Vessel").Address,
                PreviousTxId = blueprintId,
                Data = currentRunActionData.ToJsonDocument()
            };

            await _actionServiceClient.StartEvents();
            _actionServiceClient.OnConfirmed += ProcessEvent;

            // Subscribe to all wallets apart from operator
            // Ignore Operator because they get notified about all transactions
            foreach (var wallet in _wallets.Where(w => w.Name != "ExternalDataOutput"))
            {
                await _actionServiceClient.SubscribeWallet(wallet.Address!);
            }

            Console.WriteLine($"\n\tConfirmed Start Action : {startAction.Description}");
            var tx = await _actionServiceClient.Submission(actionSubmit);
            Console.WriteLine($"Submitted Action {startAction.Title} with TxId : {tx.Id}");
            Console.WriteLine($"Activity type {actionSubmit.Data.RootElement.EnumerateObject().First(o => o.Name.Contains("activitytype", StringComparison.OrdinalIgnoreCase)).Value.GetString()}");

            //Console.WriteLine(">>>>>> Press key to exit <<<<<<");
            //var input = Console.ReadKey();

            while (true)
            {

            }
        }

        private async Task NextRun()
        {
            _runIndex += 1;

            if (_runIndex + 1 > blueprintRuns.Count())
            {
                Console.WriteLine("Finished import.");
                return;
            }

            Console.WriteLine($"Moving onto run number {_runIndex + 1} of {blueprintRuns.Count()}");
            var startAction = await _actionServiceClient.GetAction(_wallets.First(w => w.Name == "Anchor Handling Vessel").Address!, _register.Id, blueprintId);

            var currentRun = blueprintRuns.ElementAt(_runIndex);
            currentRunData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(currentRun);
            var currentRunActionData = currentRunData[startAction.Title];

            var actionSubmit = new ActionSubmission
            {
                BlueprintId = blueprintId,
                RegisterId = _register.Id,
                WalletAddress = _wallets.First(w => w.Name == "Anchor Handling Vessel").Address,
                PreviousTxId = blueprintId,
                Data = currentRunActionData.ToJsonDocument()
            };

            var tx = await _actionServiceClient.Submission(actionSubmit);
            Console.WriteLine($"Processed Action {startAction.Title} on TxId : {tx.Id}");
        }

        public async Task ProcessEvent(TransactionConfirmed txData)
        {
            if (txData.ToWallets.Count == 0)
            {
                Console.WriteLine($"Transaction confirmed, received but there were no ToWallets, " +
                                  $"sent from wallet {_wallets.Find(w => w.Address == txData.Sender).Name}");

                return;
            }
            else
            {
                Console.WriteLine($"Transaction confirmed, received in wallet {_wallets.Find(w => w.Address == txData.ToWallets.First()).Name}, " +
                                  $"sent from wallet {_wallets.Find(w => w.Address == txData.Sender).Name}");
            }


            var nextAction = await _actionServiceClient.GetAction(txData.ToWallets.First(), txData.MetaData.RegisterId, txData.TransactionId);
            Console.WriteLine($"Next action is {nextAction.Title}");

            var nextActionData = currentRunData[nextAction.Title];

            var blueprintNextAction = blueprint.Actions.First(a => a.Title == nextAction.Title);
            var sendingWalletId = blueprintNextAction.Sender;

            var sendingWalletAddress = blueprint.Participants.First(p => p.Id == sendingWalletId).WalletAddress;

            var actionSubmit = new ActionSubmission
            {
                BlueprintId = blueprintId,
                RegisterId = _register.Id,
                WalletAddress = sendingWalletAddress,
                PreviousTxId = txData.TransactionId,
                Data = nextActionData.ToJsonDocument()
            };

            var tx = await _actionServiceClient.Submission(actionSubmit);
            Console.WriteLine($"Submitted Action {nextAction.Title} with TxId : {tx.Id} Sent from wallet {sendingWalletAddress}");
            //Console.WriteLine($"Activity type {actionSubmit.Data.RootElement.EnumerateObject().First(o => o.Name.Contains("activitytype", StringComparison.OrdinalIgnoreCase)).Value.GetString()}");
            Console.WriteLine("Waiting for confirmation...");

            // Don't get confirmation of last action because it doesn't go to any wallet, so check for the penultimate action
            var actions = blueprint.Actions.ToList();
            actions.Reverse();
            var penultimateAction = actions.Skip(1).Take(1).First();

            if (nextAction.Title == penultimateAction.Title)
            {
                await NextRun();
            }
        }

        public void Dispose()
        {

        }
    }
}
