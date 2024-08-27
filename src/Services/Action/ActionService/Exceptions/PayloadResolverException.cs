using System;
using System.Net;

namespace ActionService.Exceptions
{
    public class ActionResolverException : Exception
    {
        public ActionResolverException(string message) : base(message)
        {
        }
    }
}
