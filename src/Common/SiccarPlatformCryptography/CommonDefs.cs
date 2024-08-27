// Common Interfaces File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System;
using System.Linq;
#nullable enable

namespace Siccar.Platform.Cryptography
{
	public enum Status
	{
		STATUS_OK,                                      // General success return for all operations

/** Transaction Errors **/
		TX_BAD_SIGNATURE,                               // Imported Tx from JSON has an invalid signature
		TX_CORRUPT_PAYLOAD,                             // Tx data has a payload size mismatch or hash failure
		TX_BAD_FORMAT,                                  // Tx JSON cannot be parsed or is missing its build capability
		TX_NOT_SUPPORTED,                               // Operation is not supported by the Tx version
		TX_NOT_SIGNED,                                  // Requesting a field that is only valid after the Tx has been signed
		TX_INVALID_PREVTX_HASH,                         // Previous Tx hash has not been set or is invalid
		TX_SIGNING_FAILURE,                             // Various failures during signing of the transaction
		TX_SERIALIZE_FAILURE,                           // Multiple failures whilst serializing the transaction
		TX_VERIFY_FAILURE,                              // No valid signing module available for verification
		TX_NO_PAYLOAD,                                  // Tx has no payloads
		TX_BAD_METADATA,                                // MetaData is not valid JSON
		TX_INVALID_WALLET,                              // Key address is invalid or corrupt
		TX_INVALID_KEY,                                 // A private or a public key is invalid
		TX_BAD_PAYLOAD_ID,                              // If an Id of a payload does not exist
		TX_NOT_ENCRYPTED,                               // Set when accessing a payload that is not encrypted
		TX_ACCESS_DENIED,                               // When a payload cannot be accessed with a specified key
		TX_BAD_CRYPTOBOX,                               // Returned if an exception is thrown accessing or creating a cryptobox
		TX_PAYLOAD_PROTECTED,                           // Error value when trying to perform a restricted operation on a protected payload
		TX_CRYPTO_FAILURE,								// Returned when either a encrpytion/decryption/compression/decompression fails
		TX_PAYLOAD_IS_SPARSE,                           // Using an operation that is prohibited on a Sparse Transaction

/** KeyManager Errors **/
		KM_BAD_MNEMONIC,                                // Returned during KeySet recovery and the mnemonic is invalid or corrupt
		KM_GENERATE_FAIL,                               // If a crypto failure or exception occurs when generating a new KeySet
		KM_PASSWORD_FAIL,                               // The supplied password is null or incorrect
		KM_DUPLICATE_KEYRING,                           // KeyRing already exists on the KeyChain
		KM_EMPTY_KEYCHAIN,                              // When exporting a KeyChain that contains no keyrings
		KM_CRYPTO_FAILURE,								// Failure during a compression or cryptography operation
		KM_BAD_KEYRING,									// Adding a null or invalid KeyRing to a KeyChain
		KM_UNKNOWN_KEYRING,								// Remove a KeyRing that does not exist on the KeyChain.

/** Docket Errors **/
		DK_INVALID_DOCKET_HASH,
		DK_BAD_TX_HASH,
		DK_INVALID_REGISTER_ID,
		DK_INCOMPLETE_DATA,
		DK_BAD_JSON_PROPERTY,
		DK_BAD_DOCKET_FORMAT
	}
	public static class TxDefs
	{
		public const string NullTxId = "0000000000000000000000000000000000000000000000000000000000000000";
		public const UInt32 TxHashSize = 0x20;
		public const UInt32 MinTxSize = 0x5e;
		public const UInt32 SHA256HashSize = 0x20;
	}
	public enum AsymmetricMethod : UInt32
	{
		USE_SW_IMPL,
		USE_HW_HSM
	}
	public enum TransportIdentifier : UInt32
	{
		Transaction		= (UInt32)1 << 31,
		Docket			= (UInt32)1 << 30,
		Configuration	= (UInt32)1 << 29,
		Document		= (UInt32)1 << 28,
		Broadcast		= (UInt32)1 << 27,
		Consensus		= (UInt32)1 << 26
	}
	public enum PayloadFlags : UInt16
	{
		PAYLOAD_COMPRESSION = 1,
		PAYLOAD_ENCRYPTION	= 1 << 1,
		PAYLOAD_LINKED		= 1 << 2,
		PAYLOAD_IGNORE		= 1 << 3,
		PAYLOAD_ECC			= 1 << 4,
		PAYLOAD_PROTECTED	= 1 << 5
	}
	[AttributeUsage(AttributeTargets.Field)]
	public class PubKeySizeAttribute : Attribute
	{
		internal PubKeySizeAttribute(int size) { PubKeySize = size; }
		public int PubKeySize { get; private set; }
	}
	[AttributeUsage(AttributeTargets.Field)]
	public class HashSizeAttribute : Attribute
	{
		internal HashSizeAttribute(int size) { HashSize = size; }
		public int HashSize { get; private set; }
	}
	[AttributeUsage(AttributeTargets.Field)]
	public class SymKeySizeAttribute : Attribute
	{
		internal SymKeySizeAttribute(int size) { SymKeySize = size; }
		public int SymKeySize { get; private set; }
	}
	[AttributeUsage(AttributeTargets.Field)]
	public class IVSizeAttribute : Attribute
	{
		internal IVSizeAttribute(int size) { IVSize = size; }
		public int IVSize { get; private set; }
	}
	public static class EnumExtension
	{
		public static int GetPubKeySizeAttribute(this Enum attr)
		{
			object[] at = GetAttribute(attr, typeof(PubKeySizeAttribute));
			return at.Length > 0 ? ((PubKeySizeAttribute)at.First()).PubKeySize : 0;
		}
		public static int GetHashSizeAttribute(this Enum attr)
		{
			object[] at = GetAttribute(attr, typeof(HashSizeAttribute));
			return at.Length > 0 ? ((HashSizeAttribute)at.First()).HashSize : 0;
		}
		public static int GetSymKeySizeAttribute(this Enum attr)
		{
			object[] at = GetAttribute(attr, typeof(SymKeySizeAttribute));
			return at.Length > 0 ? ((SymKeySizeAttribute)at.First()).SymKeySize : 0;
		}
		public static int GetIVSizeAttribute(this Enum attr)
		{
			object[] at = GetAttribute(attr, typeof(IVSizeAttribute));
			return at.Length > 0 ? ((IVSizeAttribute)at.First()).IVSize : 0;
		}
		private static object[] GetAttribute(Enum attr, Type T) { return attr.GetType().GetMember(attr.ToString())[0].GetCustomAttributes(T, false); }
	}
	public enum WalletNetworks : byte
	{
		[PubKeySize(0x20)]
		ED25519		= 0x12,
		[PubKeySize(0x20e)]
		RSA4096		= 0x17,
		[PubKeySize(0x42)]
		NISTP256	= 0x18
	}
	public enum HashType : UInt16
	{
		[HashSize(0x20)]
		SHA256 = 0,
		[HashSize(0x30)]
		SHA384 = 1,
		[HashSize(0x40)]
		SHA512 = 2,
		[HashSize(0x20)]
		Blake2b_256 = 3,
		[HashSize(0x40)]
		Blake2b_512 = 4
	}
	public enum EncryptionType : UInt16
	{
		None = 1,
		[SymKeySize(16), IVSize(16)]
		AES_128 = 2,
		[SymKeySize(32), IVSize(16)]
		AES_256 = 3,
		[SymKeySize(32), IVSize(12)]
		AES_GCM = 4,
		[SymKeySize(32), IVSize(8)]
		CHACHA20_POLY1305 = 5,
		[SymKeySize(32), IVSize(24)]
		XCHACHA20_POLY1305 = 6
	}
	public enum CompressionType : UInt16
	{
		None		= 0,
		Max			= 1,
		Balanced	= 2,
		Fast		= 3
	}
	public enum PayloadTypeField : UInt16
	{
		Unknown		= 0,
		Docket		= 1 << 0,
		Transaction = 1 << 1,
		Rejection	= 1 << 2,
		Blueprint	= 1 << 3,
		Action		= 1 << 4,
		Document	= 1 << 5,
		Production	= 1 << 6,
		Challenge	= 1 << 7,
		Participant = 1 << 8,
		Genesys		= 1 << 9
	}
	public static class JSONKeys
	{
		public const string TxId = "TxId";
		public const string Type = "Type";
		public const string Version = "Version";
		public const string Sender = "SenderWallet";
		public const string Recipients = "RecipientsWallets";
		public const string TimeStamp = "TimeStamp";
		public const string MetaData = "MetaData";
		public const string Signature = "Signature";
		public const string PayloadCount = "PayloadCount";
		public const string Payloads = "Payloads";
		public const string Access = "WalletAccess";
		public const string Flags = "PayloadFlags";
		public const string Options = "PayloadOptions";
		public const string Challenges = "Challenges";
		public const string Wallet = "Wallet";
		public const string Network = "Network";
		public const string Size = "size";
		public const string IV = "IV";
		public const string Hex = "hex";
		public const string Data = "Data";
		public const string Hash = "Hash";
		public const string PayloadSize = "PayloadSize";
		public const string Encrypted = "Encrypted";
		public const string Compressed = "Compressed";
		public const string Protected = "Protected";
		public const string PrevId = "PrevTxId";
		public const string Register = "RegisterId";
		public const string TypeField = "TypeField";
		public const string UserField = "UserField";
		public const string QTxId = "\"" + TxId + "\":";
		public const string QType = "\"" + Type + "\":";
		public const string QVersion = "\"" + Version + "\":";
		public const string QSender = "\"" + Sender + "\":";
		public const string QRecipients = "\"" + Recipients + "\":";
		public const string QTimeStamp = "\"" + TimeStamp + "\":";
		public const string QMetaData = "\"" + MetaData + "\":";
		public const string QSignature = "\"" + Signature + "\":";
		public const string QPayloadCount = "\"" + PayloadCount + "\":";
		public const string QPayloads = "\"" + Payloads + "\":";
		public const string QAccess = "\"" + Access + "\":";
		public const string QFlags = "\"" + Flags + "\":";
		public const string QOptions = "\"" + Options + "\":";
		public const string QChallenges = "\"" + Challenges + "\":";
		public const string QWallet = "\"" + Wallet + "\":";
		public const string QNetwork = "\"" + Network + "\":";
		public const string QSize = "\"" + Size + "\":";
		public const string QIV = "\"" + IV + "\":";
		public const string QHex = "\"" + Hex + "\":";
		public const string QData = "\"" + Data + "\":";
		public const string QHash = "\"" + Hash + "\":";
		public const string QPayloadSize = "\"" + PayloadSize + "\":";
		public const string QEncrypted = "\"" + Encrypted + "\":";
		public const string QCompressed = "\"" + Compressed + "\":";
		public const string QProtected = "\"" + Protected + "\":";
		public const string QPrevId = "\"" + PrevId + "\":";
		public const string QRegister = "\"" + Register + "\":";
		public const string QTypeField = "\"" + TypeField + "\":";
		public const string QUserField = "\"" + UserField + "\":";
	}
	public class JSONValues
	{
		public readonly string Transaction = "\"Transaction\":";
		public readonly string Docket = "\"Docket\":";
	}
}
