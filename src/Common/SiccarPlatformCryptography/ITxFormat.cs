/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

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
