using System;
#nullable enable

namespace Siccar.Registers.Core
{
	public class RegisterDoesNotExistException : Exception
	{
		public RegisterDoesNotExistException(string RegisterName) {}
	}
}
