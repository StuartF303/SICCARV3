using System;
#nullable enable

namespace Siccar.Registers.Core.Storage
{
	public class RegisterRespositoryException : Exception
    {
        public override string Message { get; }

        public RegisterRespositoryException(string message) : base() 
        {
            this.Message = message;
        }
    }
}
