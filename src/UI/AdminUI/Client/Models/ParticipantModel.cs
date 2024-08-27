using System;

namespace Siccar.UI.Admin.Models
{
    /// <summary>
    /// A Participant is defined by a Friendly Name and a Wallet Address
    /// </summary>
    public class ParticipantModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Organisation { get; set; } = "";
        public string WalletAddress { get; set; } = "";
        public string didUri { get; set; } = "";
        public bool useStealthAddress { get; set; } = false;
        public bool IsFirstRow { get; set; } = false;
    }
}

