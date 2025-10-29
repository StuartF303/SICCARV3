// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

namespace Siccar.SDK.Fluent.Models
{
    /// <summary>
    /// Exception thrown when fluent builder validation fails
    /// </summary>
    public class FluentValidationException : Exception
    {
        public FluentValidationException(string message) : base(message)
        {
        }

        public FluentValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
