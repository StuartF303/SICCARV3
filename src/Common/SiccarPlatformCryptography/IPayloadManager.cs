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

// Payload Manager Interface File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public interface IPayloadManager
	{
		public void Initialize();
		public UInt32 GetPayloadsCount();
		public (Status result, IPayloadContainer[]? payloads) GetAllPayloads();
		public (Status result, IPayloadContainer[]? payloads) GetAccessiblePayloads(string? wallet = null);
		public (Status result, IPayloadContainer[]? payloads) GetInaccessiblePayloads(string? wallet = null);
		public (Status result, byte[][]? data) GetAccessiblePayloadsData(string? key = null);
		public (Status result, bool[]? ids) ImportPayloads(IPayloadContainer[]? container);
		public (Status result, UInt32[]? ids) ReleasePayloads(string? key);
		public (Status result, UInt32 id, bool[]? wallets) AddPayload(byte[]? data, string[]? wallets = null, IPayloadOptions? options = null);
		public (Status result, IPayloadContainer? payload) GetPayload(UInt32 id);
		public (Status result, byte[]? data) GetPayloadData(UInt32 id, string? key = null);
		public (Status result, UInt32 id) ImportPayload(IPayloadContainer? container);
		public Status RemovePayload(UInt32 id);
		public (Status result, IPayloadInfo? info) GetPayloadInfo(UInt32 id);
		public (Status result, IPayloadInfo[]? info) GetPayloadsInfo();
		public (Status result, bool[]? wallets) AddPayloadWallets(UInt32 id, string[]? wallets, string? key = null, EncryptionType EType = EncryptionType.None);
		public (Status result, bool added) AddPayloadWallet(UInt32 id, string? wallet, string? key = null, EncryptionType EType = EncryptionType.None);
		public (Status result, bool[]? wallets) RemovePayloadWallets(UInt32 id, string[]? wallets);
		public (Status result, bool removed) RemovePayloadWallet(UInt32 id, string? wallet);
		public Status ReleasePayload(UInt32 id, string? key, bool uncompress = true);
		public Status ReplacePayloadData(UInt32 id, byte[]? data);
		public (Status result, bool[]? verification) VerifyAllPayloadsData();
	}
}
