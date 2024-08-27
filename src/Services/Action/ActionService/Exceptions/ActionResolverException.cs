using System;
using System.Net;

namespace ActionService.Exceptions
{
    public class PayloadResolverException : Exception
    {
        public PayloadResolverException(string message) : base(message)
        {
        }
    }
}
