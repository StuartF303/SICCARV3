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
