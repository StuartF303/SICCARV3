// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Common.ServiceClients
{
    public interface IFileServiceClient : ISiccarServiceClient
    {
        public Task<List<UploadResult>> UploadFile(MultipartFormDataContent content);
        public Task RemoveFile(string fileName);
        public Task<Stream> GetFileFromTransaction(string walletAddress, string registerId, string transactionId);
    }
}
