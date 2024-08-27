using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Schema.Generation;
#nullable enable

namespace Siccar.Application
{
    /// <summary>
    /// The action purpose is to define what data 
    /// is recieved to the participant 
    /// and is returned as a reponse 
    /// for the next Participant in the flow
    /// Validation is performed by Validation/ActionValidator
    /// </summary>

    public class Action
    {
        /// <summary>
        /// The Action ID is the Transaction ID that contains it 
        /// </summary>
        [JsonPropertyName("id")]
        [MaxLength(64)]
        public int Id { get; set; } = 0;

        /// <summary>
        /// The previous TxId that was used to generate this action
        /// </summary>
        [JsonPropertyName("previousTxId")]
        [MaxLength(64)]
        public string PreviousTxId { get; set; } = string.Empty;

        /// <summary>
        /// Blueprint TX Id this action belongs to
        /// This maintains the 
        /// </summary>
        [JsonPropertyName("blueprint")]
        [MaxLength(64)]
        public string Blueprint { get; set; } = string.Empty;

        /// <summary>
        /// A useful title for this Action i.e. Apply, Endorse
        /// </summary>
        [JsonPropertyName("title")]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Usefull words for this Action i.e. Your applciation will require the following data...
        /// </summary>
        [JsonPropertyName("description")]
        [MaxLength(2048)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Address of the sender, which may be a stealth/derived addres
        /// </summary>
        [JsonPropertyName("sender")]
        [MaxLength(64)]
        public string Sender { get; set; } = string.Empty;

        /// <summary>
        /// Potential Participant to enact this step, this would involve branching/routing
        /// Condition is Principal is the WalletAddress of the Paritipant
        /// Criteria is under what conditions they get the dates  
        /// </summary>
        public IEnumerable<Condition>? Participants { get; set; } = new List<Condition>();

        /// <summary>
        /// A list of recipients that should exlude the sender of the resolved next action
        /// A string is the ID of the participant to send the transaction to.
        /// </summary>
        public IEnumerable<string> RequiredActionData { get; set; } = new List<string>();
        
        /// <summary>
        /// A list of recipients that should exlude the sender of the resolved next action
        /// A string is the ID of the participant to send the transaction to.
        /// </summary>
        [MinLength(1)]
        public IEnumerable<string> AdditionalRecipients { get; set; } = new List<string>();

        /// <summary>
        /// List of Disclosure ID's associated with SenderID
        /// Dictionary is DisclosureID an JSONLogic rule
        /// The principal here is the Data Item to be released
        /// Disclosure is under what conditions they get the dates  
        /// </summary>
        [MinLength(1)]
        public IEnumerable<Disclosure> Disclosures { get; set; } = new List<Disclosure>();


        /// <summary>
        /// JSON of Data Elements to Received by this Participant
        /// </summary>
        public JsonDocument? PreviousData { get; set; }

        /// <summary>
        /// JSON of Data Elements to Pass to the next Participant
        /// </summary>
        //public JsonDocument SubmittedData { get; set; }

        /// <summary>
        /// JSONSchema of Data Elements to Pass to the next Participant
        /// </summary>
        [MinLength(1)]
        public IEnumerable<JsonDocument>? DataSchemas { get; set; }

        /// <summary>
        /// An Action Condition resolves to a number which is the next Action
        /// Evaluated near the end by Json.Logic
        /// </summary>
        public JsonNode? Condition { get; set; } = "{ \"==\":[0,0]}";

        /// <summary>
        /// Holds user defined calculations that will be performed on submitted data
        /// Evaluated near the end by Json.Logic
        /// </summary>
        public Dictionary<string, JsonNode>? Calculations { get; set; } = new Dictionary<string, JsonNode>();

        /// <summary>
        /// Specifies the format of the data presentation
        /// </summary>
        public Control? Form { get; set; } = new Control()
        {
            ControlType = ControlTypes.Layout,
            Layout = LayoutTypes.VerticalLayout
        };


    }
}
