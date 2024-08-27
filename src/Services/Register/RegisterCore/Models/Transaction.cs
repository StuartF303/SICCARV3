// Transaction Class File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
#nullable enable

namespace Siccar.Platform
{
	[Serializable]
	public class Transaction
	{
		[DataMember]
		[Key]
		[BsonId]
		[RegularExpression("^[a-fA-F0-9]{64}$", ErrorMessage = "Hex Values Only")]
		public string? Id => TxId;
		public string? TxId { get; set; }
		[DataMember]
		[StringLength(36, ErrorMessage = "RegisterId is 38 Chars")]
		[RegularExpression("[({]?[a-fA-F0-9]{8}[-]?([a-fA-F0-9]{4}[-]?){3}[a-fA-F0-9]{12}[})]?", ErrorMessage = "GUID values only")]
		public string? RegisterId { get; set; }
		[DataMember]
		[Required]
		public byte[]? Data { get; set; }
	}
}
