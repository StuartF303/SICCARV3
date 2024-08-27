// SiccarValidator Interface File 
// Wallet Services (Siccar)

using Microsoft.Extensions.Hosting;
using System.Text.Json;
#nullable enable

namespace Siccar.Registers.ValidatorCore
{
    /// <summary>
    /// A Background service which processes Siccar Validation functionallity
    /// </summary>
    public interface ISiccarValidator : IHostedService
    {
        /// <summary>
        /// Number of registers handled by this validator
        /// </summary>
        public uint RegistersCount { get; }

        public JsonElement Status { get; }
    }
}
