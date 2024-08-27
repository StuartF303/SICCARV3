using System.Collections.Generic;
using Json.Schema.Generation;
#nullable enable

namespace Siccar.Application
{
    public class Disclosure
    {
        /// <summary>
        /// Particicpant Address : The Participant Address 
        /// </summary>
        [MaxLength(64)]
        public string ParticipantAddress { get; set; } = string.Empty;

        /// <summary>
        /// Data Elements being Accessed
        /// </summary>
        [MinItems(1)]
        public List<string> DataPointers { get; set; } = new List<string>();

        public Disclosure() {}

        public Disclosure(string participantAddress, List<string> dataPointer)
        {
            ParticipantAddress = participantAddress;
            DataPointers = dataPointer;
        }
    }
}
