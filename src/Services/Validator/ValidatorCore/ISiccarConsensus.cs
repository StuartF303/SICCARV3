// SiccarValidator Interface File 
// Wallet Services (Siccar)

using Microsoft.Extensions.Hosting;
using Siccar.Platform;
using System;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Registers.ValidatorCore
{
    /// <summary>
    /// A Background service which processes Writes Agreed updates to the Register
    /// </summary>
    public interface ISiccarConsensus : IHostedService
    {
        /// <summary>
        /// Just a property
        /// </summary>
        public bool NetworkMaster { get; }

        
    }
}
