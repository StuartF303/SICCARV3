// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using System;
using System.Net;
#nullable enable

namespace Siccar.Common.Exceptions
{
    public class HttpStatusException : Exception
    {
        public HttpStatusCode Status { get; private set; }
        public string? TraceId { get; }

        public HttpStatusException(HttpStatusCode status, string message, string? traceId = null, Exception? innerException = null) : base(message, innerException)
        {
            Status = status;
            TraceId = traceId;
        }
    }
}
