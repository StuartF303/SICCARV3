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

using Microsoft.AspNetCore.SignalR.Client;
using Siccar.Application.Models;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Siccar.Common.ServiceClients
{
    public class FileServiceClient : IFileServiceClient
    {
        private readonly SiccarBaseClient _baseClient;
        private readonly string _endpoint = "files";

        public string? ConnectionId { get; set; }

        public FileServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }

        public async Task<List<UploadResult>> UploadFile(MultipartFormDataContent content)
        {
            var response = await _baseClient.PostJsonAsync($"{_endpoint}", content);
            var responseData = JsonSerializer.Deserialize<List<UploadResult>>(response, _baseClient.serializerOptions);
            return responseData!;
        }

        public async Task<Stream> GetFileFromTransaction(string walletAddress, string registerId, string transactionId)
        {
            var response = await _baseClient.GetStream($"{_endpoint}/transactions/{walletAddress}/{registerId}/{transactionId}");
            return response;
        }

        public async Task RemoveFile(string fileName)
        {
            await _baseClient.Delete($"{_endpoint}/{fileName}");
        }
    }
}
