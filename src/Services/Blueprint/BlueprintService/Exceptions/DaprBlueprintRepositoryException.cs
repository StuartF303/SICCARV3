using System;
using System.Net;

namespace BlueprintService.Exceptions
{
    public class DaprBlueprintRepositoryException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public DaprBlueprintRepositoryException(HttpStatusCode status, string message) : base(message)
        {
            Status = status;
        }
    }
}
