// PayloadContainer Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public interface IPayloadContainer
	{
		public UInt32 GetPayloadId();
		public IPayload GetPayload();
	}
}
