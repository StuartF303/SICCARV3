
namespace Siccar.Platform
{
	public class PayloadModel
	{
		public string[] WalletAccess { get; set; }
		public ulong PayloadSize { get; set; }
		public string Hash { get; set; }
		public string Data { get; set; }
		public string PayloadFlags { get; set; }
		public Challenge IV { get; set; }
		public Challenge[] Challenges { get; set; }
	}

	public class Challenge
	{
		public string hex { get; set; }
		public ulong size { get; set; }
	}
}
