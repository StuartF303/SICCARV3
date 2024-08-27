using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Siccar.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

#nullable enable
namespace Siccar.UI.Admin.Models
{
    public class DataSchemaProperty
    {

        public DataSchemaProperty()
        {
        }

        public DataSchemaProperty(DataSchemaProperty dataSchemaProperty)
        {
            Id = dataSchemaProperty.Id;
            Type = dataSchemaProperty.Type;
            Format = dataSchemaProperty.Format;
            Title = dataSchemaProperty.Title;
            Description = dataSchemaProperty.Description;
            Default = dataSchemaProperty.Default;
            MinLength = dataSchemaProperty.MinLength;
            MaxLength = dataSchemaProperty.MaxLength;
            Required = dataSchemaProperty.Required;
            Enum = dataSchemaProperty.Enum;
            Disclosures = dataSchemaProperty.Disclosures.ToList();
        }

        public void AddFileProperites()
        {
            var fileName = new DataSchemaProperty()
            {
                Id = "fileName",
                Description = "The name of the file uploaded.",
                Type = "string"
            };
            var fileType = new DataSchemaProperty()
            {
                Id = "fileType",
                Description = "The type of the file uploaded.",
                Type = "string"
            };
            var fileSize = new DataSchemaProperty()
            {
                Id = "fileSize",
                Description = "The size of the file uploaded in bytes.",
                Type = "integer"
            };
            RequiredProperties = new List<string> { fileName.Id, fileType.Id, fileSize.Id };
            Properties = new() { { "fileName", fileName }, { "fileType", fileType }, { "fileSize", fileSize } };
        }

        [JsonPropertyName("$id")]
        [Required]
        public string Id { get; set; } = "";
        [JsonPropertyName("type")]
        [Required]
        public string? Type { get; set; }

        /// <summary>
        /// The property type based on the ControlType
        /// </summary>
        [JsonIgnore]
        public string? PropertyType
        {
            get
            {
                if (Type == null) return null;
                if (Type == ControlTypes.DateTime.ToString())
                {
                    //TODO Don't do this here
                    Format = "date-time";
                }
                if (System.Enum.TryParse(Type, out ControlTypes controlType))
                {
                    return GetPropertyTypeFromControlType(controlType);
                }
                return Type; //Fallback to type value
            }
        }

        [JsonPropertyName("format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Format { get; set; }

        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
        [JsonPropertyName("default")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public dynamic? Default { get; set; }
        [JsonPropertyName("minLength")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinLength { get; set; }
        [JsonPropertyName("maxLength")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxLength { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public bool Required { get; set; }
        [JsonPropertyName("required")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? RequiredProperties { get; set; }
        [JsonPropertyName("enum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Enum { get; set; }
        [JsonPropertyName("properties")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, DataSchemaProperty>? Properties { get; set; }
        [IgnoreDataMember]
        [JsonIgnore]
        public List<string> Disclosures { get; set; } = new List<string>();

        private static string GetPropertyTypeFromControlType(ControlTypes controlType)
        {
            return controlType switch
            {
                ControlTypes.Layout => "",
                ControlTypes.Label => "string",
                ControlTypes.TextLine => "string",
                ControlTypes.TextArea => "string",
                ControlTypes.Numeric => "integer",//TODO decimal/float/etc
                ControlTypes.DateTime => "string",
                ControlTypes.File => "object",
                ControlTypes.Choice => "string",
                ControlTypes.Checkbox => "boolean",
                ControlTypes.Selection => "string",
                _ => throw new ArgumentOutOfRangeException(nameof(controlType), controlType, null),
            };
        }
    }


}
