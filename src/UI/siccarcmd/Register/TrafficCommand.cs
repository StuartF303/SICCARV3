using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;

namespace siccarcmd.register
{
    public class TrafficCommand : Command
    {
        public RegisterServiceClient commshandler = null;

        public TrafficCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "traffic";
            this.Description = "Generates some backgound traffic for a Register Service";

            Add(new Argument<string>("name", description: "A name of the Register to use."));
            Add(new Option<int>(new string[] { "-m", "--minutes" }, getDefaultValue: () => 10, description: "How long to run for in minutes."));
            Add(new Option<int>(new string[] { "-w", "--wait" }, () => 5, description: "Wait between generated transactions."));
            Add(new Option<int>(new string[] { "-c", "--count" }, () => 0, description: "Create a specific number of transactions, defaults to continuous "));
            Add(new Option<bool>(new string[] { "-j", "--json" }, () => false, description: "Pretty print the transaction JSON"));

            commshandler = services.GetService<ISiccarServiceClient>() as RegisterServiceClient;
            Handler = CommandHandler.Create<string, int, int, int, bool, int>(CreateNoise);
        }

        private void CreateNoise(string name, int minutes, int wait, int count, bool json, int port = 5004)
        {
            //commshandler.Port = port;

            //JsonSerializerOptions opts = new JsonSerializerOptions( JsonSerializerDefaults.Web)
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    WriteIndented = true
            //};
            //// run for 10 minutes
            //var starttime = DateTime.UtcNow;
            //Random r = new Random();

            //int rounds = 0;

            //Register regState = commshandler.GetRegisterState(name).Result;


            //if (json) // print the start state
            //{
            //    var regString = JsonSerializer.Serialize(regState, opts);
            //    Console.WriteLine("Register State : \n {0}", regString);
            //}

            //ulong startDocket = regState.Height;

            //Docket docket = new Docket();
            //bool fastbreak = false;

            //do
            //{
            //    List<Transaction> trans = new List<Transaction>();
            //    // generate 1 to 10 transactions with a random interval 1-5 seconds
            //    int txs = 0;

            //    if (count > 0)
            //        txs = count;
            //    else
            //        txs = r.Next(1, 11);

            //    for (int i = 0; i < txs; i++)
            //    {
            //        var tx = TransGenerate(name);
            //        trans.Add(tx);
            //        var resp = commshandler.PostNewTransaction(tx).Result;
            //        var delay = (r.Next(1000, wait * 1000) / 1000);
            //        Thread.Sleep(delay);

            //        if (json)
            //        {
            //            var txString = JsonSerializer.Serialize(tx, opts);
            //            Console.WriteLine("New Tx Id : {0} :: \n {1}", resp, txString);
            //        }
            //        else
            //            Console.WriteLine("New Tx : {0} : {1}", resp, tx.TxId);

            //    }

            //    // do a consensus round
            //    docket = DocketGen(startDocket++, trans, docket.PreviousHash);
            //    var respDk = commshandler.PostNewDocket(docket).Result;

            //    if (json)
            //    {
            //        var dxString = JsonSerializer.Serialize(docket, opts);
            //        Console.WriteLine("New Docket Id : {0} :: \n {1}", respDk, dxString);
            //    }
            //    else
            //        Console.WriteLine("New Docket Id : {0} : {1} ", respDk, docket.Id);
              

            //    if ((count > 0) && (rounds++ >= count))
            //        fastbreak = true; // break out of the while
            //}
            //while ((DateTime.UtcNow.Subtract(starttime).Minutes < minutes) && fastbreak);

        }



        //private Transaction TransGenerate(string Register)
        //{
        //    Random r = new Random();

        //    var tx = new Transaction();
        //    tx.RegisterId = Register;
        //    tx.FromWallet = NewAddress();
        //    tx.ToWallet = new List<string>() { NewAddress() };
        //    tx.State = TransactionState.Pending;
        //    tx.TransactionType = TransactionTypes.Section;

        //    tx.PayloadCount = (ulong)r.Next(10, 30);
        //    tx.Payloads = RandomPayload();

        //    tx.TxId = ComputeSha256Hash(tx.FromWallet);

        //    return tx;
        //}



        //public static Docket DocketGen(ulong id, List<Transaction> txs, string prevhash)
        //{
        //    Docket dkt = new Docket();

        //    dkt.Id = id+1;
        //    dkt.PreviousHash = prevhash;
        //    dkt.RegisterId = txs.First().RegisterId;
        //    dkt.Timestamp = DateTime.UtcNow;
        //    dkt.State = DocketState.Sealed;
        //    dkt.TransactionIds = txs.Select(t => t.TxId).ToList();
        //    dkt.Votes = ComputeSha256Hash(dkt.Id.ToString());
        //    dkt.Hash = ComputeSha256Hash(dkt.Id.ToString());
        //    dkt.PreviousHash = ComputeSha256Hash(dkt.Id.ToString());

        //    return dkt;
        //}

        //public static string ComputeSha256Hash(string rawData)
        //{
        //    // Create a SHA256   
        //    using (SHA256 sha256Hash = SHA256.Create())
        //    {
        //        // ComputeHash - returns byte array  
        //        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        //        // Convert byte array to a string   
        //        StringBuilder builder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            builder.Append(bytes[i].ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}


        //private static string NewAddress()
        //{
        //    // actually in base 58 we have restricted chars so for the moment going to be even more restrictive
        //    Random r = new Random();
        //    var s = new StringBuilder();
        //    int length = r.Next(26, 35);

        //    for(int i = 0; i < length; i++)
        //    {
        //        int rc = r.Next(0, 54); // 
        //        char c;
        //        //[RegularExpression("^[a-km-zA-HJ-NP-Z1-9]{26,35}$"
        //        // 1-9 : 49-57 : 8
        //        // A-H : 64-72 : 8
        //        // J-N : 74-78 : 4
        //        // P-Z : 80-90 : 10
        //        // a-k : 97-107 : 10
        //        // m-z : 109-122 : 13
        //        if (rc < 9)
        //            c = (char)(rc + 49);
        //        else if (rc < 17)
        //            c = (char)(rc + 64 - 8); // 65 is the offset for A  
        //        else if (rc < 22)
        //            c = (char)(rc + 74 - 16); // 74 is the offset for J 
        //        else if (rc < 32)
        //            c = (char)(rc + 80 - 20) ; // 65 is the offset for A 
        //        else if (rc < 42)
        //            c = (char)(rc + 97 - 30); // 65 is the offset for A 
        //        else  
        //            c = (char)(rc + 109 - 40); // 65 is the offset for A 

        //        s.Append(c);
        //    }

        //    //[RegularExpression("^[a-km-zA-HJ-NP-Z1-9]{26,35}$"
        //    return s.ToString();
        //}

        //private static Payload[] RandomPayload()
        //{
        //    //ToDo: Generate a fake payload.
        //    return new Payload[] { new Payload() };
        //}
    }
}

