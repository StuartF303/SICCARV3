// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Siccar.Application;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating Blueprint participants
    /// </summary>
    public class ParticipantBuilder
    {
        private readonly Participant _participant;

        internal ParticipantBuilder(string id)
        {
            _participant = new Participant
            {
                Id = id
            };
        }

        /// <summary>
        /// Sets the participant's name
        /// </summary>
        /// <param name="name">The participant's friendly name</param>
        /// <returns>The builder instance for chaining</returns>
        public ParticipantBuilder Named(string name)
        {
            _participant.Name = name;
            return this;
        }

        /// <summary>
        /// Sets the participant's organisation
        /// </summary>
        /// <param name="organisation">The organisation name</param>
        /// <returns>The builder instance for chaining</returns>
        public ParticipantBuilder FromOrganisation(string organisation)
        {
            _participant.Organisation = organisation;
            return this;
        }

        /// <summary>
        /// Sets the participant's wallet address
        /// </summary>
        /// <param name="walletAddress">The wallet address (e.g., ws1...)</param>
        /// <returns>The builder instance for chaining</returns>
        public ParticipantBuilder WithWallet(string walletAddress)
        {
            _participant.WalletAddress = walletAddress;
            return this;
        }

        /// <summary>
        /// Sets the participant's DID URI
        /// </summary>
        /// <param name="didUri">The DID URI</param>
        /// <returns>The builder instance for chaining</returns>
        public ParticipantBuilder WithDidUri(string didUri)
        {
            _participant.didUri = didUri;
            return this;
        }

        /// <summary>
        /// Configures the participant to use a stealth/privacy address
        /// </summary>
        /// <param name="useStealth">Whether to use stealth address (default: true)</param>
        /// <returns>The builder instance for chaining</returns>
        public ParticipantBuilder UseStealthAddress(bool useStealth = true)
        {
            _participant.useStealthAddress = useStealth;
            return this;
        }

        internal Participant Build() => _participant;
    }
}
