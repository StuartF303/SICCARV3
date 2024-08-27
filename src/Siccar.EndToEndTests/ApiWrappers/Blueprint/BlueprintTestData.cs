using Siccar.Application;

namespace Siccar.EndToEndTests.Blueprint
{
    public class BlueprintTestData
    {
        public const string DefaultBlueprintTitle = "Test Blueprint";
        public const string DefaultBlueprintDescription = "A test Blueprint";

        public static Models.Blueprint NewDefault(string senderWalletAddress,
            string receiverWalletAddress)
        {
            var blueprintId = Guid.NewGuid().ToString();
            return new Models.Blueprint
            {
                Id = blueprintId,
                Title = DefaultBlueprintTitle,
                Description = DefaultBlueprintDescription,
                Version = 1,
                Participants = new List<dynamic>
                {
                    new
                    {
                        walletAddress = senderWalletAddress,
                        name = "Sender",
                        id = Guid.NewGuid(),
                        organisation = "Siccar"
                    },
                    new
                    {
                        walletAddress = receiverWalletAddress,
                        name = "Receiver",
                        id = Guid.NewGuid(),
                        organisation = "Siccar"
                    }
                },
                Actions = new List<Models.Action>
                {
                    new()
                    {
                        Id = 1,
                        Title = "First Action",
                        Sender = senderWalletAddress,
                        Disclosures = new List<Disclosure>
                        {
                            new()
                            {
                                ParticipantAddress = receiverWalletAddress,
                                DataPointers = new List<string> { "name", "surname" }
                            }
                        },
                        Blueprint = blueprintId,
                        PreviousTxId = "000000000000000000000000000000000",
                        DataSchemas = new List<dynamic>
                        {
                            new Dictionary<string, object>
                            {
                                { "$schema", "http://json-schema.org/draft-07/schema" },
                                { "type", "object" },
                                { "title", "Simple Data" },
                                { "description", "A Simple data item between participants" },
                                {
                                    "properties", new
                                    {
                                        name = new Dictionary<string, object>
                                            { { "$id", "name" }, { "type", "string" }, { "title", "First Name" } },
                                        surname = new Dictionary<string, object>
                                            { { "$id", "surname" }, { "type", "string" }, { "title", "Surname" } },

                                    }
                                },
                            }
                        }
                    },

                }
                
            };
        }
    }
}
