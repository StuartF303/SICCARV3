using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WalletService.Core
{
    public class WalletException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public WalletException(HttpStatusCode errorCode, string message) : base(message)
        {
            Status = errorCode;
        }
    }
}
