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

// Payload Manager V4 Class Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
#nullable enable

namespace Siccar.Platform.Cryptography.TransactionV4
{

/*******************************************************************************\
* PayloadContainer()															*
* This class is used for exporting and importing payloads into a transaction.	*
* It contains a complete payload object and the id for the payload within the	*
* transaction used for export. The id is not relevant to the import process.	*
\*******************************************************************************/
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
		private enum AccessType { ACCESSIBLE, INACCESSIBLE }
		private readonly List<IPayload> tx_payloads = [];
		private readonly CryptoModule CryptoModule;
		private bool tx_modified = true;

/*******************************************************************************\
* PayloadManager()																*
* This private constructor is the only available method for instantiating the	*
* PayloadManager. It takes a provided CryptoModule as its argument which is		*
* only made available to a constructed payload. The CryptoModule may be			*
* customised as per the requirements of a payload.								*
\*******************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051: Reflection invoked constructor")]
		private PayloadManager(CryptoModule module) { CryptoModule = module; }

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
* function will succeed even if no payloads have been added to the Transaction, *
* or if it is signed or unsigned.												*
\*******************************************************************************/
		public UInt32 GetPayloadsCount() { return (UInt32)tx_payloads.Count; }

/*******************************************************************************\
* GetAllPayloads()																*
* Returns a value for the number of payloads currently in the Transaction. This *
* function will succeed and return an empty array even if no payloads have been	*
* added to the Transaction. Only payloads that are not protected or sparse will	*
* be returned. This method is valid for both signed or unsigned transactions.	*
\*******************************************************************************/
		public (Status result, IPayloadContainer[]? payloads) GetAllPayloads()
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (tx_payloads[0].DataSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, null);
			List<IPayloadContainer> payloads = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				if (!tx_payloads[i].DataProtected())
					payloads.Add(new PayloadContainer((UInt32)i + 1, tx_payloads[i]));
			}
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
			{
				IPayloadInfo? info = tx_payloads[i].GetInfo();
				if (info == null)
					return (Status.TX_CORRUPT_PAYLOAD, null);
				payload_info[i] = info;
			}
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
			(Status result, IPayloadInfo[]? info) = GetPayloadsInfo();
			if (result != Status.STATUS_OK || info == null || info.Length != tx_payloads.Count)
				return (result, null);
			if (info[0].GetPayloadSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, null);
			List<byte[]> access_data = [];
			for (int i = 0; i < info.Length; i++)
			{
				(Status status, byte[]? data) = GetDataPayload(tx_payloads[i], key);
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
* payload import will have its index resturned in this array. This method		*
* processes all provided payloads. If the transaction is sparse, this method	*
* will fail and not import any payloads. Recipients order for payloads is		*
* ignored and all imported payloads will have their hash validated.				*
\*******************************************************************************/
		public (Status result, bool[]? ids) ImportPayloads(IPayloadContainer[]? payloads)
		{
			if (payloads == null || payloads.Length < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (tx_payloads.Count > 0 && tx_payloads[0].DataSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, null);
			bool[] id = new bool[payloads.Length];
			for (int i = 0; i < payloads.Length; i++)
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
		public (Status result, bool[]? wallets) AddPayloadWallets(UInt32 id, string[]? wallets, string? key = null, EncryptionType EType = EncryptionType.XCHACHA20_POLY1305)
		{
			Status status = PayloadCheck(id);
			if (status != Status.STATUS_OK)
				return (status, null);
			if (wallets == null || wallets.Length < 1)
				return (Status.TX_INVALID_WALLET, null);
			if (tx_payloads[(int)id - 1].DataEncrypted() && (key == null || key.Length < 1))
				return (Status.TX_INVALID_KEY, null);
			(bool[] Valid, WalletUtils.CryptoKey[]? Wallets) = WalletUtils.WalletsCheck(wallets);
			if (Wallets != null && Wallets.Length > 0)
			{
				(status, bool[]? wlist) = AddWallet(tx_payloads[(int)id - 1], key, Wallets, EType);
				if (status != Status.STATUS_OK || wlist == null)
					return (status, null);
				for (int i = 0, j = 0; i < Valid.Length; i++)
				{
					if (Valid[i])
					{
						Valid[i] = wlist![j++];
						if (Valid[i])
							tx_modified = true;
					}
				}
			}
			return (Status.STATUS_OK, Valid);
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
* All the supplied wallets are processed by this method and as such the reason	*
* for an individual wallet not being removed, is not returned.					*
\*******************************************************************************/
		public (Status result, bool[]? wallets) RemovePayloadWallets(UInt32 id, string[]? wallets)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			Status status = PayloadCheck(id);
			if (status != Status.STATUS_OK)
				return (status, null);
			if (wallets == null || wallets.Length < 1)
				return (Status.TX_INVALID_WALLET, null);
			if (!tx_payloads[(int)id - 1].DataEncrypted())
				return (Status.TX_NOT_ENCRYPTED, null);
			(bool[] Valid, WalletUtils.CryptoKey[]? Wallets) = WalletUtils.WalletsCheck(wallets);
			if (Wallets != null && Wallets.Length > 0)
			{
				(Status result, bool[]? wlist) = RemoveWallet(tx_payloads[(int)id - 1], Wallets);
				if (result != Status.STATUS_OK || wlist == null)
					return (Status.TX_INVALID_WALLET, null);
				for (int i = 0, j = 0; i < Valid.Length; i++)
				{
					if (Valid[i])
					{
						Valid[i] = wlist[j++];
						if (Valid[i])
							tx_modified = true;
					}
				}
			}
			return (Status.STATUS_OK, Valid);
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
			if (tx_payloads == null || tx_payloads.Count < 1)
				return (Status.TX_NO_PAYLOAD, null);
			if (key == null || key.Length < 1)
				return (Status.TX_INVALID_KEY, null);
			List<UInt32> id = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				if (PayloadCheck((UInt32)i + 1) == Status.STATUS_OK && tx_payloads[i].DataEncrypted())
				{
					(Status status, bool released) = ReleaseData(tx_payloads[i], key);
					if (status == Status.STATUS_OK && released)
					{
						id.Add((UInt32)i + 1);
						tx_modified = true;
					}
				}
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
			if (tx_payloads.Count > 0 && tx_payloads[0].DataSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, 0, null);
			try
			{
				ConstructorInfo? ci = typeof(Payload).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(CryptoModule)], null);
				if (ci != null)
				{
					IPayload p = (Payload)ci.Invoke([CryptoModule]);
					MethodInfo? method = p.GetType().GetMethod("Build", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(byte[]),
						typeof(WalletUtils.CryptoKey[]), typeof(IPayloadOptions)]);
					if (method != null)
					{
						(bool[] Valid, WalletUtils.CryptoKey[]? Wallets) = WalletUtils.WalletsCheck(wallets);
						var status = method.Invoke(p, [data, Wallets!, options!]);
						if (status != null && (Status)status == Status.STATUS_OK)
						{
							tx_payloads.Add(p);
							tx_modified = true;
							return ((Status)status, (UInt32)tx_payloads.Count, Valid);
						}
					}
				}
			}
			catch (Exception) {}
			return (Status.TX_BAD_FORMAT, 0, null);
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
			Status status = PayloadCheck(id);
			return (status == Status.STATUS_OK || status == Status.TX_ACCESS_DENIED) ?
				(Status.STATUS_OK, new PayloadContainer(id, tx_payloads[(int)id - 1])) : (status, null);
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
			if (id < 1 || id > tx_payloads.Count)
				return (Status.TX_BAD_PAYLOAD_ID, null);
			else if (tx_modified)
				return (Status.TX_NOT_SIGNED, null);
			else if (tx_payloads[(int)id - 1].DataSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, null);
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
			if (tx_payloads.Count > 1 && tx_payloads[0].DataSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, 0);
			tx_payloads.Add(payload.GetPayload());
			tx_modified = true;
			return (Status.STATUS_OK, (UInt32)tx_payloads.Count);
		}

/*******************************************************************************\
* RemovePayload()																*
* This method can be used to remove a payload from a transaction using a		*
* specified id. This function will fail if the specified payload is protected,	*
* or the transaction is sparse. No other access verification is performed on	*
* the payload being removed. An error will be returned if an id outside of the	*
* range of payloads	within a transaction is specified. Removing a payload from	*
* a transaction will invalidate any cached payload id greater than the id		*
* specified.																	*
\*******************************************************************************/
		public Status RemovePayload(UInt32 id)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return Status.TX_NO_PAYLOAD;
			Status status = PayloadCheck(id);
			if (status == Status.STATUS_OK || status == Status.TX_ACCESS_DENIED)
			{
				tx_payloads.RemoveAt((int)id - 1);
				tx_modified = true;
			}
			return status;
		}

/*******************************************************************************\
* GetPayloadInfo()																*
* This method will return the PayloadInfo structure for the specified payload.	*
* Payload indexes start at 1 and may be invalidated by methods that	add or		*
* remove payloads from the transaction.											*
\*******************************************************************************/
		public (Status result, IPayloadInfo? info) GetPayloadInfo(UInt32 id)
		{
			return (tx_payloads == null || tx_payloads.Count < 1) ? (Status.TX_NO_PAYLOAD, null) :
				(id > tx_payloads.Count || id < 1) ? (Status.TX_BAD_PAYLOAD_ID, null) : (Status.STATUS_OK, tx_payloads[(int)id - 1].GetInfo());
		}

/*******************************************************************************\
* AddPayloadWallet()															*
* This method can be used to add a wallet to a specified payload. A valid key	*
* for payload access must be supplied if the payload is encrypted. If the		*
* payload is unencrypted, adding a wallet will encrypt the payload using the	*
* specified algorithm. A wallet cannot be added to a payload that is declared	*
* as sparse or protected. Added wallets are checked for correct formatting,		*
* network and duplicates. If the payload already contains the wallet to be		*
* added, then it will be discarded. Using an encryption method of				*
* EncryptionType.None on a payload that is unencrypted will result in the		*
* default encryption algorithm being used. Upon successful addition of a wallet	*
* the payload size, flags and hash will be recomputed. This method should be	*
* considered obsolete in favour of AddPayloadWallets().							*
\*******************************************************************************/
		[Obsolete("Use AddPayloadWallets() instead")]
		public (Status result, bool added) AddPayloadWallet(UInt32 id, string? wallet, string? key, EncryptionType EType = EncryptionType.XCHACHA20_POLY1305)
		{
			(Status status, bool[]? wallets) = AddPayloadWallets(id, [wallet!], key, EType);
			return (status, wallets != null && wallets.Length == 1 && wallets[0]);
		}

/*******************************************************************************\
* RemovePayloadWallet()															*
* This method removes a wallet from the specified payload. The id must identify	*
* a suitable payload and must not be protected or sparse in order for this		*
* function to succeed. It should be noted that removal of a wallet from a		*
* payload may lead to the data being permanently inaccessible. This method		*
* should be considered obsolete in favour of RemovePayloadWallets().			*
\*******************************************************************************/
		[Obsolete("Use RemovePayloadWallets() instead")]
		public (Status result, bool removed) RemovePayloadWallet(UInt32 id, string? wallet)
		{
			(Status status, bool[]? wallets) = RemovePayloadWallets(id, [wallet!] );
			return (status, wallets != null && wallets.Length == 1 && wallets[0]);
		}

/*******************************************************************************\
* ReleasePayload()																*
* This method allows the removal of all encryption from a payload specified by	*
* its id. The id must equate to a valid payload and specified key able to		*
* provide wallet access. By default, compression is also removed by the	use of	*
* this method, but this feature may be disabled by providing an appropriate		*
* argument. Releasing a payload cannot be performed on an unencrypted,			*
* protected	or sparse payload.													*
\*******************************************************************************/
		public Status ReleasePayload(UInt32 id, string? key, bool uncompress)
		{
			if (tx_payloads == null || tx_payloads.Count < 1)
				return Status.TX_NO_PAYLOAD;
			if (key == null || key.Length < 1)
				return Status.TX_INVALID_KEY;
			Status status = PayloadCheck(id);
			if (status != Status.STATUS_OK)
				return status;
			else if (!tx_payloads[(int)id - 1].DataEncrypted())
				return Status.TX_NOT_ENCRYPTED;
			(status, bool released) = ReleaseData(tx_payloads[(int)id - 1], key);
			if (status == Status.STATUS_OK && released)
				tx_modified = true;
			return status;
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
			Status status = PayloadCheck(id);
			if (status != Status.STATUS_OK)
				return status;
			(status, bool replaced) = ReplaceData(tx_payloads[(int)id - 1], data);
			if (status != Status.STATUS_OK || !replaced)
				return status;
			tx_modified = true;
			return Status.STATUS_OK;
		}

/**
 * Private methods of the PayloadManager used for getting access to the payloads
 **/

/*******************************************************************************\
* PayloadCheck()																*
* This internal method is used to check specific flags and state of a payload	*
* using	its id. If the id is valid, it will return a state identifying whether	*
* the payload is protected, sparse or inaccessible due to wallet access.		*
\*******************************************************************************/
		private Status PayloadCheck(UInt32 id)
		{
			if (id < 1 || id > tx_payloads.Count)
				return Status.TX_BAD_PAYLOAD_ID;
			IPayloadInfo? info = GetPayloadInfo(id).info;
			if (info == null)
				return Status.TX_BAD_FORMAT;
			if (info.GetPayloadProtected())
				return Status.TX_PAYLOAD_PROTECTED;
			else if (info.GetPayloadSparse())
				return Status.TX_PAYLOAD_IS_SPARSE;
			return (info.GetPayloadEncrypted() && info.GetPayloadWalletsCount() < 1) ? Status.TX_ACCESS_DENIED : Status.STATUS_OK;
		}

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
			if (wallet == null || !WalletUtils.WalletsCheck([wallet]).Valid[0])
				return (Status.TX_INVALID_WALLET, null);
			(Status result, IPayloadInfo[]? info) = GetPayloadsInfo();
			if (result != Status.STATUS_OK || info == null || info.Length != tx_payloads.Count)
				return (result, null);
			if (info[0].GetPayloadSparse())
				return (Status.TX_PAYLOAD_IS_SPARSE, null);
			List<IPayloadContainer> container = [];
			for (int i = 0; i < info.Length; i++)
			{
				string[] wallets = info[i].GetPayloadWallets();
				bool present = false;
				for (int j = 0; j < wallets.Length; j++)
				{
					if (wallets[j] == wallet)
					{
						present = true;
						break;
					}
				}
				if (
					(type == AccessType.ACCESSIBLE && (present || !info[i].GetPayloadEncrypted()) && !tx_payloads[i].DataProtected()) ||
					(type == AccessType.INACCESSIBLE && !present && info[i].GetPayloadEncrypted() && !tx_payloads[i].DataProtected()))
					container.Add(new PayloadContainer((uint)i + 1, tx_payloads[i]));
			}
			return (Status.STATUS_OK, container.ToArray());
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
		private Status BuildPayloads(BinaryReader reader, bool sparse)
		{
			try
			{
				UInt32 count = (UInt32)WalletUtils.ReadVLSize(reader);
				if (count > 0)
				{
					ConstructorInfo? ci = typeof(Payload).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(CryptoModule)], null);
					if (ci != null)
					{
						for (int i = 0; i < count; i++)
						{
							IPayload p = (Payload)ci.Invoke([CryptoModule]);
							MethodInfo? method = p.GetType().GetMethod("Build", BindingFlags.Instance | BindingFlags.NonPublic,
								[typeof(BinaryReader), typeof(bool)]);
							if (method == null)
								return Status.TX_CORRUPT_PAYLOAD;
							Status? status = (Status?)method.Invoke(p, [reader, sparse]);
							if (status == null || status != Status.STATUS_OK)
								return Status.TX_CORRUPT_PAYLOAD;
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
			List<byte> data = [];
			for (int i = 0; i < tx_payloads.Count; i++)
			{
				MethodInfo? method = tx_payloads[i].GetType()
					.GetMethod(oper == TxFormat.Operation.SIGNING ? "SigningData" : "HashingData", BindingFlags.Instance | BindingFlags.NonPublic);
				if (method == null)
					return (Status.TX_SERIALIZE_FAILURE, null);
				List<byte>? bytes = (List<byte>?)method.Invoke(tx_payloads[i], null);
				if (bytes == null || bytes.Count < 1)
					return (Status.TX_SERIALIZE_FAILURE, null);
				data.AddRange(bytes);
			}
			return (Status.STATUS_OK, data);
		}

/**
 * Methods for accessing the private functionality of the payload class. The functions
 * here provide access to methods that modify the payload. They should not be called
 * by any method outside of this class.
 **/

		private static (Status status, T? data) PayloadCall<T>(object o, string name, params object[] parms)
		{
			MethodInfo? method = o.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
			if (method != null)
			{
				(Status status, T?)? args = ((Status status, T?)?)method.Invoke(o, [.. parms]);
				if (args != null)
					return (args.Value.status, args.Value.Item2);
			}
			return (Status.TX_CORRUPT_PAYLOAD, default(T));
		}
		private static (Status result, bool[]? wallets) AddWallet(object o, string? key, WalletUtils.CryptoKey[] wallets, EncryptionType type)
		{
			return PayloadCall<bool[]>(o, "AddWallets", key!, wallets, type);
		}
		private static (Status result, bool[]? wallets) RemoveWallet(object o, WalletUtils.CryptoKey[] wallets) { return PayloadCall<bool[]>(o, "RemoveWallets", wallets); }
		private static (Status result, byte[]? data) GetDataPayload(object o, string? key) { return PayloadCall<byte[]>(o, "GetData", key!); }
		private static (Status result, bool release) ReleaseData(object o, string key) { return PayloadCall<bool>(o, "ReleaseData", key); }
		private static (Status result, bool release) ReplaceData(object o, byte[] data) { return PayloadCall<bool>(o, "ReplaceData", data); }
	}
}
