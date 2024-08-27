using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Peers.Utilities
{
    public partial class Utilities
    {
        private ILogger Log = null;
        private IConfiguration Configuration = null;

        public Utilities(ILogger logger, IConfiguration configuration)
        {
            Log = logger;
            Configuration = configuration;
        }
    }
}
