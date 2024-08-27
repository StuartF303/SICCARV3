using Json.More;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
#nullable enable

namespace Siccar.Platform
{
	public class TransactionMetaData : IEquatable<TransactionMetaData>
	{
		/// <summary>
		/// The DB Record ID for MetaDataStorage Record - Not return to Clients
		/// </summary>
		[JsonIgnore]
		[Key]
		public int Id { get; set; } = 0;

		/// <summary>
		/// This is likely a string lowercase ALPHA only
		/// </summary>
		[Required]
		[DataMember]
		[RegularExpression(@"^(\{){0,1}[0-9a-fA-F]{8}[0-9a-fA-F]{4}[0-9a-fA-F]{4}[0-9a-fA-F]{4}[0-9a-fA-F]{12}(\}){0,1}$", ErrorMessage = "Must be a guid without hyphens")]
		public string RegisterId { get; set; } = String.Empty;

		/// <summary>
		/// The type of Transaction in the Register
		/// </summary>
		[DataMember]
		public TransactionTypes? TransactionType { get; set; } = TransactionTypes.Action;

		/// <summary>
		/// The Blueprint transactionID which the data payload is based on.
		/// </summary>
		[DataMember]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string BlueprintId { get; set; } = String.Empty;

		/// <summary>
		/// Identifies a specific instance of a blueprint. i.e each first action being submitted creates a new instance id
		/// </summary>
		[DataMember]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string InstanceId { get; set; } = String.Empty;

		/// <summary>
		/// The Id of the action which is being submitted
		/// </summary>
		[DataMember]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int ActionId { get; set; } = 1;

		/// <summary>
		/// The Id of the action which should be submitted next in the blueprint
		/// </summary>
		[DataMember]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
		public int NextActionId { get; set; } = 1;

		/// <summary>
		/// MetaData that is used to track the progress of a blueprint instance
		/// </summary>
		[DataMember]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public SortedList<string, string> TrackingData
		{
			get {
				_trackingData ??= JsonSerializer.Deserialize<SortedList<string, string>>(_trackingDataJson, new JsonSerializerOptions(JsonSerializerDefaults.Web));
				return _trackingData ?? new();
			}
			set { 
				_trackingData = value ?? new();
				_trackingDataJson = JsonSerializer.Serialize(_trackingData, new JsonSerializerOptions(JsonSerializerDefaults.Web));
			}
		}

		[IgnoreDataMember]
		private SortedList<string, string>? _trackingData = new();

		/// <summary>
		/// this is the bit thats actually stored
		/// </summary>
		[IgnoreDataMember]
		[JsonIgnore]
		[BsonElement("_trackingDataJson"), BsonRequired]
		private string _trackingDataJson { get; set; } = "{}";

		public override bool Equals(object? obj)
		{
			return Equals(obj as TransactionMetaData);
		}

		public bool Equals(TransactionMetaData? other)
		{
			return other != null &&
				Id == other.Id &&
				RegisterId == other.RegisterId &&
				TransactionType == other.TransactionType &&
				BlueprintId == other.BlueprintId &&
				InstanceId == other.InstanceId &&
				ActionId == other.ActionId &&
				NextActionId == other.NextActionId &&
				EqualityComparer<SortedList<string, string>>.Default.Equals(TrackingData, other.TrackingData) &&
				EqualityComparer<SortedList<string, string>>.Default.Equals(_trackingData, other._trackingData) &&
				_trackingDataJson == other._trackingDataJson;
		}

		public override int GetHashCode()
		{
			HashCode hash = new();
			hash.Add(Id);
			hash.Add(RegisterId);
			hash.Add(TransactionType);
			hash.Add(BlueprintId);
			hash.Add(InstanceId);
			hash.Add(ActionId);
			hash.Add(NextActionId);
			hash.Add(TrackingData);
			hash.Add(_trackingData);
			hash.Add(_trackingDataJson);
			return hash.ToHashCode();
		}
	}

	/// <summary>
	/// Various states a Transaction can be in
	/// </summary>
	[JsonConverter(typeof(EnumStringConverter<TransactionTypes>))]
	public enum TransactionTypes
	{
		/// <summary>
		/// We must incude system register items
		/// </summary>
		Docket = 0,
		/// <summary>
		/// A transaction which rejects action data
		/// </summary>
		Rejection = 4,
		/// <summary>
		/// Publish a Blueprint
		/// </summary>
		Blueprint = 10,
		/// <summary>
		/// A Transaction that has data from a Blueprint
		/// </summary>
		Action = 11,
		/// <summary>
		/// An encrypted shared asset
		/// </summary>
		File = 12,
		/// <summary>
		/// A provable Open asset
		/// </summary>
		Production = 13,
		/// <summary>
		/// A Crypto Challenge
		/// </summary>
		Challenge = 14,
		/// <summary>
		/// A Published Participant Wallet
		/// </summary>
		Participant = 15
	}
}
