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

// Payload Manager V3 Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV3
{
	[Serializable]
	public class PayloadContainer(UInt32 id, IPayload p) : IPayloadContainer
	{
		private readonly IPayload payload = p;
		private readonly UInt32 payload_id = id;
		public UInt32 GetPayloadId() { return payload_id; }
		public IPayload GetPayload() { return payload; }
	}

	[Serializable]
	public class PayloadManager : IPayloadManager
	{

		private readonly List<IPayload> tx_payloads = [];
		private enum AccessType { ACCESSIBLE, INACCESSIBLE }
		private bool tx_modified = true;

		public void Initialize()
		{
			tx_payloads.Clear();
			tx_modified = true;
		}

/**
 * Payload management functions for multiple payloads
 **/

/*******************************************************************************\
* GetPayloadsCount()															*
* Returns a value for the number of payloads currently in the Transaction. This *
* function will succeed even if no payloads have been added to the Transaction. *
* The returned value is valid if the Transaction is signed or unsigned.			*
\*******************************************************************************/
		public UInt32 GetPayloadsCount() { return (UInt32)tx_payloads.Count; }

/*******************************************************************************\
* GetAllPayloads()																*
* Returns a value for the number of payloads currently in the Transaction. This *
* function will succeed and return an empty array even if no payloads have been	*
* added to the Transaction. The returned value is valid if the Transaction is	*
* signed or unsigned.															*
\*******************************************************************************/
		public (Status result, IPayloadContainer[]? payloads) GetAllPayloads()
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			List<IPayloadContainer> payloads = [];
			for (int i = 0; i < tx_payloads.Count; i++)
				payloads.Add(new PayloadContainer((UInt32)i + 1, tx_payloads[i]));
			return (Status.STATUS_OK, payloads.ToArray());
		}

/*******************************************************************************\
* GetAccessiblePayloads()														*
* This method will return an array of payloads within a transaction that the	*
* supplied wallet can access. If no key, or null is specified as an	argument,	*
* then only those payloads that are unencrypted will be returned. All payloads	*
* are returned wrapped in PayloadContainer objects. If no payload can be		*
* accessed using the specified key, then this method will return null as		*
* the container array.															*
\*******************************************************************************/
		public (Status result, IPayloadContainer[]? payloads) GetAccessiblePayloads(string? wallet = null) { return GetAccessPayloads(AccessType.ACCESSIBLE, wallet); }

/*******************************************************************************\
* GetInAccessiblePayloads()														*
* Returns all payloads that cannot be acccessed within a transaction using a	*
* specified wallet. If null is used as a wallet argument then this method will	*
* return only those payloads that are encrypted. All payloads are returned		*
* wrapped in PayloadContainer objects. If all payloads can be accessed with the	*
* specified wallet or are unencrypted then this method will return null.		*
\*******************************************************************************/
		public (Status result, IPayloadContainer[]? payloads) GetInaccessiblePayloads(string? wallet = null) { return GetAccessPayloads(AccessType.INACCESSIBLE, wallet); }

/*******************************************************************************\
* GetPayloadsInfo()																*
* Returns an array of PayloadInfo structures describing the features of the		*
* payload. This will return an empty array if no payloads have been added to	*
* the transaction. Using this method to get a each payload id, hash, data size	*
* and other information.														*
\*******************************************************************************/
		public (Status result, IPayloadInfo[]? info) GetPayloadsInfo()
		{
			IPayloadInfo[] payload_info = new IPayloadInfo[tx_payloads.Count];
			for (int i = 0; i < tx_payloads.Count; i++)
				payload_info[i] = tx_payloads[i].GetInfo()!;
			return (Status.STATUS_OK, payload_info);
		}

/*******************************************************************************\
* GetAccessiblePayloadsData()													*
* This method returns an array of byte arrays containing the data for each		*
* payload that can be accessed with the specified key. If an error occurs		*
* during payload data extraction in any payload, or the supplied key is			*
* invalid, then this method will return an error and null. It will not return	*
* successful/partial payload extractions if one fails. Payloads that cannot be	*
* accessed with the specified key are skipped and do not represent a failure	*
* condition. Calling this method with no key will extract data from unencrypted	*
* payloads only.																*
\*******************************************************************************/
		public (Status result, byte[][]? data) GetAccessiblePayloadsData(string? key = null)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (tx_modified)
				return (Status.TX_NOT_SIGNED, null);
			List<byte[]> access_data = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				IPayloadInfo? info = tx_payloads[i].GetInfo();
				if (info!.GetPayloadEncrypted())
				{
					if (key == null)
						continue;
					var (result, access) = WIFAccess(tx_payloads[i], key);
					if (result == Status.TX_ACCESS_DENIED)
						continue;
					if (result != Status.STATUS_OK || !access)
						return (result, null);
				}
				var (status, data) = GetDataPayload(tx_payloads[i], key);
				if ((status == Status.STATUS_OK || status == Status.TX_NOT_ENCRYPTED) && data != null)
					access_data.Add(data);
			}
			return (Status.STATUS_OK, access_data.ToArray());
		}

/*******************************************************************************\
* ImportPayloads()																*
* This method imports an array of payloads into the transaction. All payloads	*
* must be wrapped into PayloadContainer structures. The id of the payload is	*
* not used when importing payloads into a transaction. On completion an array	*
* of indexes is returned corresponding to the input array. Each successful		*
* payload import will have its index resturned in this array. This method will	*
* process all payloads and import only those that meet the transaction			*
* requirements. Importing a payload will not alter these transaction			*
* requirements.	Recipient order for payloads is ignored and all imported		*
* payloads will have their hash validated before import.						*
\*******************************************************************************/
		public (Status result, bool[]? ids) ImportPayloads(IPayloadContainer[]? payloads)
		{
			if (payloads == null || payloads.Length < 1)
				return (Status.TX_NO_PAYLOAD, null);
			bool[] id = new bool[payloads.Length];
			for (uint i = 0; i < payloads.Length; i++)
			{
				id[i] = payloads[i].GetPayload().VerifyHash();
				if (id[i])
				{
					tx_payloads.Add(payloads[i].GetPayload());
					tx_modified = true;
				}
			}
			return (Status.STATUS_OK, id);
		}

/*******************************************************************************\
* AddPayloadWallets()															*
* Use this method to add wallets to a specified payload. A valid key for the	*
* payload must be supplied if the payload is encrypted.	If wallets are added to	*
* an unencrypted payload, then the payload is encrypted to the specified		*
* wallets. On successful completion, an array representing the indexes of the	*
* supplied input wallets is returned, indicating which wallets were added to	*
* payload. Payloads which are already secured by a specified wallet will not be	*
* added to the payload. TX_ACCESS_DENIED will be returned if the supplied key	*
* cannot unlock the payload.													*
\*******************************************************************************/
		public (Status result, bool[]? wallets) AddPayloadWallets(UInt32 id, string[]? wallets, string? key = null, EncryptionType type = EncryptionType.AES_256)
		{
			static bool contains(bool[]? blist)
			{
				if (blist != null)
				{
					for (int i = 0; i < blist.Length; i++)
					{
						if (blist[i])
							return true;
					}
				}
				return false;
			}
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (id < 1 || id > tx_payloads.Count)
				return (Status.TX_BAD_PAYLOAD_ID, null);
			if (wallets == null || wallets.Length < 1)
				return (Status.TX_INVALID_WALLET, null);
			(Status result, bool[]? wlist) status = AddWallet(tx_payloads[(int)id - 1], wallets, key);
			if (status.wlist != null && status.wlist.Length > 0 && contains(status.wlist))
				tx_modified = true;
			return status;
		}

/*******************************************************************************\
* RemovePayloadWallets()														*
* Using this method allows the removal of wallets from a specified payload. The	*
* id of the payload must be greater than zero and less than the payload count	*
* for the transaction in order for this function to succeed. On successful		*
* completion, an index array of the input wallets specifies which wallets have	*
* been removed from the payload. It should be noted that removal of wallets		*
* from a payload does not remove the encryption of the payload data. A Payload	*
* may be rendered inaccessible if all wallets are removed using this function.	*
\*******************************************************************************/
		public (Status result, bool[]? wallets) RemovePayloadWallets(UInt32 id, string[]? wallets)
		{
			static bool contains(bool[]? blist)
			{
				if (blist != null)
				{
					for (int i = 0; i < blist.Length; i++)
					{
						if (blist[i])
							return true;
					}
				}
				return false;
			}
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (id < 1 || id > tx_payloads.Count)
				return (Status.TX_BAD_PAYLOAD_ID, null);
			if (wallets == null || wallets.Length < 1)
				return (Status.TX_INVALID_WALLET, null);
			(Status result, bool[]? wlist) status = RemoveWallet(tx_payloads[(int)id - 1], wallets);
			if (status.wlist != null && status.wlist.Length > 0 && contains(status.wlist))
				tx_modified = true;
			return status;
		}

/*******************************************************************************\
* ReleasePayloads()																*
* Use this method to remove the encryption from any payload that the specified	*
* key can access. The specified key may not be null or this function will fail.	*
* This method can only be used on a signed transaction. On successful			*
* completion, this method will return a list of payload ids that were decrypted	*
* by the operation. EXTREME CARE must be taken when using this function as it	*
* performs in-place payload decryption. Should decryption fail on a payload, it	*
* should be assumed that the transaction is in an invalid state. This method	*
* will stop processing payloads if a decryption	or other operational error		*
* occurs and return an appropriate status error.								*
\*******************************************************************************/
		public (Status result, UInt32[]? ids) ReleasePayloads(string? key)
		{
			if (tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (key == null || key.Length < 1)
				return (Status.TX_INVALID_KEY, null);
			if (tx_modified)
				return (Status.TX_NOT_SIGNED, null);
			List<UInt32> id = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				var (result, access) = WIFAccess(tx_payloads[i], key);
				if (result == Status.TX_ACCESS_DENIED || result == Status.TX_NOT_ENCRYPTED)
					continue;
				if (result != Status.STATUS_OK || !access)
					return (result, null);
				Status status = DecryptData(tx_payloads[i], key);
				if (status != Status.STATUS_OK)
					return (status, null);
				id.Add((UInt32)i + 1);
				tx_modified = true;
			}
			return (Status.STATUS_OK, id.ToArray());
		}

/*******************************************************************************\
* VerifyAllPayloadsData()														*
* Use this method to verify the data integrity of all of the payloads within a	*
* transaction. It requires no access to any of the payloads of the transaction	*
* and verifies that the data of the payload has a matching hash.				*
\*******************************************************************************/
		public (Status result, bool[]? verification) VerifyAllPayloadsData()
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			bool[] verify = new bool[tx_payloads.Count];
			for (int i = 0; i < tx_payloads.Count; i++)
				verify[i] = tx_payloads[i].VerifyHash();
			return (Status.STATUS_OK, verify);
		}

/**
 * Payload management functions for single payloads
 **/

/*******************************************************************************\
* AddPayload()																	*
* This method takes the data supplied as an argument and creates a payload		*
* structure which is stored within the transaction.	The wallets argument array	*
* defines which wallets the data is secured to. Duplicate and invalid wallets	*
* will be filtered when creating the payload. An array defining accepted		*
* wallets will be returned by this method. The Id of the new payload within the	*
* is also returned.																*
\*******************************************************************************/
		public (Status result, UInt32 id, bool[]? wallets) AddPayload(byte[]? data, string[]? wallets = null, IPayloadOptions? options = null)
		{
			if (data == null || data.Length < 1)
				return (Status.TX_NO_PAYLOAD, 0, null);
			bool[]? ids = null;
			HashSet<string> parsed = [];
			if (wallets != null && wallets.Length > 0)
			{
				ids = new bool[wallets.Length];
				for (int i = 0; i < wallets.Length; i++)
				{
					(byte Network, byte[] PubKey)? w = WalletUtils.WalletToPubKey(wallets[i]);
					ids[i] = w != null && (WalletNetworks)w.Value.Network == WalletNetworks.ED25519 && parsed.Add(wallets[i]);
				}
				wallets = [.. parsed];
			}
			ConstructorInfo? ci = typeof(Payload).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(byte[]), typeof(string[]), typeof(IPayloadOptions)], null);
			Status status = Status.TX_CORRUPT_PAYLOAD;
			if (ci != null)
			{
				IPayload? p = (Payload?)ci.Invoke([data, wallets!, options!]);
				if (p != null)
				{
					status = LastError(p);
					if (status == Status.STATUS_OK)
					{
						tx_payloads.Add(p);
						tx_modified = true;
						return (status, (UInt32)tx_payloads.Count, ids);
					}
				}
			}
			return (status, 0, null);
		}

/*******************************************************************************\
* GetPayload()																	*
* Used to obtain a specific payload from a transaction using a supplied id. All	*
* transaction Ids are indexed from 1. Using an index of zero, or outside of the	*
* range contained within the transaction will return a TX_NO_PAYLOAD error.	Any	*
* payload returned by this function will be wrapped in a IPayloadContainer.		*
* This transaction version does NOT support locked payloads that cannot be		*
* extracted.																	*
\*******************************************************************************/
		public (Status result, IPayloadContainer? payload) GetPayload(UInt32 id)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			return (id > tx_payloads.Count || id < 1) ? (Status.TX_BAD_PAYLOAD_ID, null) : (Status.STATUS_OK, new PayloadContainer(id, tx_payloads[(int)id - 1]));
		}

/*******************************************************************************\
* GetPayloadData()																*
* This method will extract the payload data from a specified payload. If the	*
* supplied key																	*
* range contained within the transaction will return a TX_NO_PAYLOAD error.	Any	*
* payload returned by this function will be wrapped in a IPayloadContainer.		*
* This transaction version does NOT support locked payloads that cannot be		*
* extracted.																	*
\*******************************************************************************/
		public (Status result, byte[]? data) GetPayloadData(UInt32 id, string? key = null)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (id <= 0 || id > tx_payloads.Count)
				return (Status.TX_BAD_PAYLOAD_ID, null);
			if (tx_modified)
				return (Status.TX_NOT_SIGNED, null);
			var (result, access) = WIFAccess(tx_payloads[(int)id - 1], key);
			if (!access && result != Status.TX_NOT_ENCRYPTED)
				return (result, null);
			return GetDataPayload(tx_payloads[(int)id - 1], key);
		}

/*******************************************************************************\
* ImportPayload()																*
* Use this method to import a payload into a transaction. The payload must be	*
* wrapped in a PayloadContainer object. On successful addition to the			*
* transaction, the id of the payload within the transaction is returned. The	*
* payload must have a valid hash in order to be added to the transaction. This	*
* method provides a more detailed result than ImportPayloads() for a given		*
* import operation and should be the preferred method for adding payloads to a	*
* transaction.																	*
\*******************************************************************************/
		public (Status result, UInt32 id) ImportPayload(IPayloadContainer? payload)
		{
			if (payload == null)
				return (Status.TX_NO_PAYLOAD, 0);
			if (!payload.GetPayload().VerifyHash())
				return (Status.TX_CORRUPT_PAYLOAD, 0);
			tx_payloads.Add(payload.GetPayload());
			tx_modified = true;
			return (Status.STATUS_OK, (UInt32)tx_payloads.Count);
		}

/*******************************************************************************\
* RemovePayload()																*
* This method can be used to remove a payload from a transaction using a		*
* specified id. No access verification is performed on the payload being		*
* removed. An error will be returned if an id outside of the range of payloads	*
* within a transaction is used. Removing a payload from a transaction will		*
* invalidate any cached payload id greater than the id specified.				*
\*******************************************************************************/
		public Status RemovePayload(UInt32 id)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return Status.TX_NO_PAYLOAD;
			if (id < 1 || id > tx_payloads.Count)
				return Status.TX_BAD_PAYLOAD_ID;
			tx_payloads.RemoveAt((int)id - 1);
			tx_modified = true;
			return Status.STATUS_OK;
		}

/*******************************************************************************\
* GetPayloadInfo()																*
* This method will return the PayloadInfo structure of the specified index.		*
* Note that payload indexes start at 1 and may be invalidated by methods that	*
* add or remove payloads from the transaction.									*
\*******************************************************************************/
		public (Status result, IPayloadInfo? info) GetPayloadInfo(UInt32 id)
		{
			return (tx_payloads == null || tx_payloads.Count < 1) ? (Status.TX_NO_PAYLOAD, null) :
				(id > tx_payloads.Count || id < 1) ? (Status.TX_BAD_PAYLOAD_ID, null) : (Status.STATUS_OK, tx_payloads[(int)id - 1].GetInfo());
		}

/*******************************************************************************\
* AddPayloadWallet()															*
* This method can be used to add wallets to a specified payload. A valid key	*
* for the payload must be supplied if the payload is encrypted.	A wallet added	*
* to an unencrypted payload will encrypt the data to that specified wallet.		*
* If the id specified is zero or out of the payload range of the transaction,	*
* then this method will fail with an appropriate error. The transaction must	*
* contain at least one valid payload for this function to succeed. If the		*
* wallet is added to the payload then this method will return true. No			*
* operation will take place if the payload is already secured to the specified	*
* wallet.																		*
\*******************************************************************************/
		public (Status result, bool added) AddPayloadWallet(UInt32 id, string? wallet, string? key, EncryptionType EType)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, false);
			if (id < 1 || id > tx_payloads.Count)
				return (Status.TX_BAD_PAYLOAD_ID, false);
			if (wallet == null || wallet.Length < 1 || WalletUtils.WalletToPubKey(wallet) == null)
				return (Status.TX_INVALID_WALLET, false);
			(Status result, bool[]? wallets) = AddWallet(tx_payloads[(int)id - 1], [wallet], key);
			bool added = wallets != null && wallets[0];
			if (added)
				tx_modified = true;
			return (result, added);
		}

/*******************************************************************************\
* RemovePayloadWallet()															*
* This method removes a supplied wallet from a specific payload. The id of the	*
* payload must be greater than zero and less than the payload count	for the		*
* transaction in order for this function to succeed. If successful, the method	*
* returns true.	It should be noted that removal of a wallet from a payload may	*
* lead to the data being permanently inaccessible.								*
\*******************************************************************************/
		public (Status result, bool removed) RemovePayloadWallet(UInt32 id, string? wallet)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, false);
			if (id < 1 || id > tx_payloads.Count)
				return (Status.TX_BAD_PAYLOAD_ID, false);
			if (wallet == null || wallet.Length < 1 || WalletUtils.WalletToPubKey(wallet) == null)
				return (Status.TX_INVALID_WALLET, false);
			if (!tx_payloads[(int)id - 1].GetInfo()!.GetPayloadEncrypted())
				return (Status.TX_NOT_ENCRYPTED, false);
			(Status result, bool[]? wallets) = RemoveWallet(tx_payloads[(int)id - 1], [wallet]);
			bool removed = wallets != null && wallets[0];
			if (removed)
				tx_modified = true;
			return (result, removed);
		}

/*******************************************************************************\
* ReleasePayload()																*
* This method allows the removal of all encryption from a payload specified by	*
* its id. The id must be greater than zero, less than the payload count of the	*
* transaction and the key must also be able to access the payload in order to	*
* succeed. If the payload cannot be accessed with the provided key, then this	*
* function will return TX_ACCESS_DENIED.										*
\*******************************************************************************/
		public Status ReleasePayload(UInt32 id, string? key, bool uncompress)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return Status.TX_NO_PAYLOAD;
			if (id < 1 || id > tx_payloads.Count)
				return Status.TX_BAD_PAYLOAD_ID;
			(Status status, bool access) = WIFAccess(tx_payloads[(int)id - 1], key);
			if (status == Status.STATUS_OK && access)
			{
				DecryptData(tx_payloads[(int)id - 1], key);
				tx_modified = true;
			}
			return LastError(tx_payloads[(int)id - 1]);
		}

/*******************************************************************************\
* ReplacePayloadData()															*
* This method provides functionality to completely change the contents of a		*
* transaction payload. The newly addded payload data will conform to the access	*
* specification of the transaction. No access is required to the payload being	*
* replaced. The id specifies the payload of the transaction to target and must	*
* be greater than zero and less than the payload count in order to succeed.		*
\*******************************************************************************/
		public Status ReplacePayloadData(UInt32 id, byte[]? data)
		{
			if (tx_payloads == null || tx_payloads.Count < 1 || data == null || data.Length < 1)
				return Status.TX_NO_PAYLOAD;
			if (id < 1 || id > tx_payloads.Count)
				return Status.TX_BAD_PAYLOAD_ID;
			MethodInfo? method = tx_payloads[(int)id - 1].GetType().GetMethod("ReplaceData", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
				return Status.TX_CORRUPT_PAYLOAD;
			bool result = (bool?)method.Invoke(tx_payloads[(int)id - 1], [data]) ?? false;
			Status status = LastError(tx_payloads[(int)id - 1]);
			if (status == Status.STATUS_OK && result)
				tx_modified = true;
			return status;
		}

/**
 * Private methods of the PayloadManager used for getting access to the payloads
 **/

/*******************************************************************************\
* GetAccessPayloads()															*
* Used for returning a list of payloads that match specified access				*
* requirements. This can be payloads that have a access by a specific wallet or	*
* paylods that cannot be accessed by that wallet. If no wallet is specified		*
* then this function will target unencrypted payloads.							*
\*******************************************************************************/
		private (Status result, IPayloadContainer[]? container) GetAccessPayloads(AccessType type, string? wallet = null)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			List<IPayloadContainer> container = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				Status status = Status.TX_INVALID_WALLET;
				MethodInfo? method = tx_payloads[i].GetType().GetMethod("WalletAccess", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method != null)
				{
					bool access = (bool?)method.Invoke(tx_payloads[i], [wallet!]) ?? false;
					status = LastError(tx_payloads[i]);
					if (status != Status.TX_INVALID_WALLET || status == Status.TX_NOT_ENCRYPTED)
					{
						if ((type == AccessType.ACCESSIBLE && access) || (type == AccessType.INACCESSIBLE && !access))
							container.Add(new PayloadContainer((uint)i + 1, tx_payloads[i]));
						continue;
					}
				}
				return (status, null);
			}
			return (Status.STATUS_OK, container.Count > 0 ? container.ToArray() : null);
		}

/*******************************************************************************\
* TxModified()																	*
* Returns the status of whether the transaction payloads have been modified.	*
* In the case of transaction signing, this function is called to set the state	*
* that a transaction is unmodified at that time. ONLY signing of the			*
* transaction should be used to fix the modified flag of the paylods. All		*
* methods within this class that modify transaction state should reset this		*
* flag.																			*
\*******************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked method")]
		private bool TxModified(bool IsModified = false)
		{
			if (IsModified)
				tx_modified = false;
			return tx_modified;
		}

/*******************************************************************************\
* BuildPayloads()																*
* This method builds payloads for the transaction from a binary input stream.	*
* Supports building of both signed and unsigned transaction payloads.			*
\*******************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked method")]
		private Status BuildPayloads(BinaryReader reader)
		{
			try
			{
				UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
				if (count > 0)
				{
					ConstructorInfo? ci = typeof(Payload).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(BinaryReader)], null);
					if (ci != null)
					{
						for (uint i = 0; i < count; i++)
						{
							WalletUtils.ReadVLSize(reader);
							IPayload p = (IPayload)ci!.Invoke([reader]);
							if (LastError(p) != Status.STATUS_OK)
								return LastError(p);
							tx_payloads.Add(p);
							tx_modified = true;
						}
						return Status.STATUS_OK;
					}
				}
			}
			catch (Exception) {}
			return Status.TX_CORRUPT_PAYLOAD;
		}

/*******************************************************************************\
* SerializePayloads()															*
* Used to serialize payloads into a binary stream for both signing and			*
* transaction Id calculation. The transaction must contain at least one payload	*
* in order for this function to succeed.										*
\*******************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked method")]
		private (Status result, List<byte>? data) SerializePayloads(TxFormat.Operation oper)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			List<byte> data = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				MethodInfo? method = tx_payloads[i].GetType()
					.GetMethod(oper == TxFormat.Operation.SIGNING ? "SigningData" : "HashingData", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method == null)
					return (Status.TX_SERIALIZE_FAILURE, null);
				List<byte>? bytes = (List<byte>?)method.Invoke(tx_payloads[i], null);
				if (bytes != null && bytes.Count > 0)
					data.AddRange(bytes);
			}
			return (Status.STATUS_OK, data);
		}

/**
 * Methods for accessing the private functionality of the payload class. The functions
 * here provide access to methods that modify the payload. They should not be called
 * by any method outside this class.
 **/
		private static (Status result, bool access) WIFAccess(object o, string? key)
		{
			MethodInfo? method = o.GetType().GetMethod("WIFAccess", BindingFlags.Instance | BindingFlags.NonPublic);
			(Status status, bool access) box = (Status.TX_INVALID_KEY, false);
			if (method != null)
			{
				box.access = (bool?)method.Invoke(o, [key!]) ?? false;
				box.status = LastError(o);
			}
			return box;
		}
		private static (Status result, bool[]? wallets) AddWallet(object o, string[] wallets, string? key = null)
		{
			MethodInfo? method = o.GetType().GetMethod("AddWallets", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
				return (Status.TX_INVALID_WALLET, null);
			bool[]? w = (bool[]?)method.Invoke(o, [key!, wallets]);
			return (LastError(o), w);
		}
		private static (Status result, bool[]? wallets) RemoveWallet(object o, string[] wallets)
		{
			MethodInfo? method = o.GetType().GetMethod("RemoveWallets", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
				return (Status.TX_INVALID_WALLET, null);
			bool[]? w = (bool[]?)method.Invoke(o, [wallets]);
			return (LastError(o), w);
		}
		private static (Status result, byte[]? data) GetDataPayload(object o, string? key)
		{
			MethodInfo? method = o.GetType().GetMethod("GetData", BindingFlags.Instance | BindingFlags.NonPublic);
			(Status status, byte[]? data) pdata;
			if (method != null)
			{
				pdata.data = (byte[]?)method.Invoke(o, [key!]);
				pdata.status = LastError(o);
				return pdata;
			}
			return (Status.TX_CORRUPT_PAYLOAD, null);
		}
		private static Status DecryptData(object o, string? key)
		{
			MethodInfo? method = o.GetType().GetMethod("DecryptData", BindingFlags.Instance | BindingFlags.NonPublic);
			return (method == null) ? Status.TX_CORRUPT_PAYLOAD : (bool?)method.Invoke(o, [key!]) ?? false ? Status.STATUS_OK : LastError(o);
		}
		private static Status LastError(object o)
		{
			MethodInfo? method = o.GetType().GetMethod("GetLastError", BindingFlags.Instance | BindingFlags.NonPublic);
			return (method != null) ? (Status)method.Invoke(o, null)! : Status.TX_CORRUPT_PAYLOAD;
		}
	}
}
