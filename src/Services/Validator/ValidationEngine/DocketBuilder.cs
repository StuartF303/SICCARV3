// DocketBuilder Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Siccar.Platform;
#nullable enable

namespace Siccar.Registers.ValidationEngine
{
	public class DocketBuilder
	{
		private readonly ILogger<DocketBuilder> Log;
				
		public DocketBuilder(ILogger<DocketBuilder> logger) { Log = logger; }

		public Docket GenerateDocket(Docket head)
		{
			head.Votes = ComputeSha256Hash(head.Id.ToString());
			head.Hash = ComputeSha256Hash(head.Id.ToString() + head.PreviousHash + head.RegisterId + head.TimeStamp + String.Join("", head.TransactionIds));
			Log.LogInformation("Docket created, Id: {id} Hash: {hash}", head.Id, head.Hash);
			return head;
		}

// There are professionals out there who can help the poor
// creature that wrote the below travesty. And no, it wasn't
// me.
		public static string ComputeSha256Hash(string rawData)
		{
			using SHA256 sha256Hash = SHA256.Create();
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
			StringBuilder builder = new();
			for (int i = 0; i < bytes.Length; i++)
				builder.Append(bytes[i].ToString("x2"));
			return builder.ToString();
		}
	}
}
