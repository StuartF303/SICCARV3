// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
#nullable enable
namespace Siccar.Application.Models
{
    public class FileMetaData
    {
        [JsonPropertyName("fileName")]
        public string? Name { get; set; }
        [JsonPropertyName("fileType")]
        public string? Type { get; set; }
        [JsonPropertyName("fileExtension")]
        public string? Extension { get; set; }
        [JsonPropertyName("fileSize")]
        public long? Size { get; set; }
        [JsonPropertyName("fileTransactionId")]
        public string? TransactionId { get; set; }
    }
}
