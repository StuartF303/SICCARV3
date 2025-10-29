// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

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
