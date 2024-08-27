// Transaction Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public interface ITxFormat
	{
		public UInt32 GetTxVersion();
		public (Status result, string? hash) GetPrevTxHash();
		public (Status result, string? hash) GetTxHash();
		public (Status result, string? sender) GetTxSender();
		public (Status result, string[] recipients) GetTxRecipients();
		public (Status result, string? metadata) GetTxMetaData();
		public (Status result, string? timestamp) GetTxTimeStamp();
		public (Status result, string? signature) GetTxSignature();
		public Status SetPrevTxHash(string? hash);
		public (Status result, bool[]? recipients) SetTxRecipients(string[]? recipients);
		public Status SetTxMetaData(string? meta);
		public Status SignTx(string? privkey);
		public Status VerifyTx();
		public (Status result, Transaction? transport) GetTxTransport();
		public (Status result, string? json) ToJSON();
		public IPayloadManager GetTxPayloadManager();
	}
}
