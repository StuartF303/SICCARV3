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
using Siccar.Platform;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using static Siccar.Common.ServiceClients.IRegisterServiceClient;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Siccar.Common.ServiceClients
{


    public class RegisterServiceClient : IRegisterServiceClient
    {
        private readonly SiccarBaseClient _baseClient;
        private readonly string _endpoint = "Registers";
        private HubConnection? _hubConnection;

        public RegisterServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }

        // Specifics
        public async Task<List<TransactionModel>> GetAllTransactionsByBlueprintId(string registerId, string blueprintTxId)
        {
            var instanceQuery = $"?$filter=MetaData/blueprintId eq '{blueprintTxId}'";
            var URI = $"{_endpoint}/{registerId}/transactions" + instanceQuery;
            var response = await _baseClient.GetJsonAsync(URI);
            return JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;
        }

        public async Task<TransactionModel> GetTransactionById(string registerId, string transactionId)
        {
            var URI = $"{_endpoint}/{registerId}/transactions/{transactionId}";
            var response = await _baseClient.GetJsonAsync(URI);
            return JsonSerializer.Deserialize<TransactionModel>(response, _baseClient.serializerOptions)!;
        }

        public async Task<List<TransactionModel>> GetAllTransactions(string registerId, string query = "")
        {
            var uri = $"{_endpoint}/{registerId}/transactions{query}";
            var response = await _baseClient.GetJsonAsync(uri);
            return response.Deserialize<List<TransactionModel>>(_baseClient.serializerOptions)!;
        }
        public async Task<ODataRaw<List<TransactionModel>>> GetAllTransactionsOData(string registerId, string query = "")
        {
            var uri = $"{_endpoint}({registerId})/transactions{query}";
            var response = await _baseClient.GetJsonAsyncOData(uri);
            return response.Deserialize<ODataRaw<List<TransactionModel>>>(_baseClient.serializerOptions)!;
        }

        public async Task<string> PostNewTransaction(string register, TransactionModel data)
        {
            var requestPath = $"{_endpoint}/{register}/transactions";
            var jdata = JsonSerializer.Serialize<TransactionModel>(data);
            var response = await _baseClient.PostJsonAsync(requestPath, jdata);
            return $"StatusCode: {response}";
        }

        public async Task<string> PostNewDocket(Docket data)
        {
            var requestPath = $"{_endpoint}/{data.RegisterId}/Dockets";
            var jdata = JsonSerializer.Serialize(data);
            var response = await _baseClient.PostJsonAsync(requestPath, jdata);
            return $"StatusCode: {response}";
        }

        public async Task<Docket> GetDocketByHeight(string register, UInt64 Height)
        {
            var requestPath = $"{_endpoint}/{register}/Dockets/{Height}";
            var response = await _baseClient.GetJsonAsync(requestPath);
            if (response != null)
            {
                var dkt = JsonSerializer.Deserialize<Docket>(response, _baseClient.serializerOptions);
                if (dkt != null)
                    return dkt;
            }
            return new Docket();
        }

        public async Task<Register> CreateRegister(Register data)
        {
            var jdata = JsonSerializer.Serialize<Register>(data, _baseClient.serializerOptions);
            var response = await _baseClient.PostJsonAsync(_endpoint, jdata);
            return JsonSerializer.Deserialize<Register>(response, _baseClient.serializerOptions)!;
        }

        // add a delete function - requires register.creator 
        public async Task<bool> DeleteRegister(string registerId)
        {
            var path = _endpoint + "/" + registerId;
            try
            {
                var response = await _baseClient.Delete(path);
                return true;
            }
            catch (Exception er)
            {
                _baseClient.Log.LogError("Cannot delete register : {RegisterId} : Error : {Message}", registerId, er.Message);
                return false;
            }

        }

        public async Task<List<Register>> GetRegisters()
        {
            var response = await _baseClient.GetJsonAsync(_endpoint);
            if (response != null)
            {
                var listReg = JsonSerializer.Deserialize<List<Register>>(response, _baseClient.serializerOptions);
                if (listReg != null)
                    return listReg;
            }
            return new List<Register>();
        }

        public async Task<Register> GetRegister(string registerId)
        {
            var requestPath = $"{_endpoint}/{registerId}";
            var register = (await _baseClient.GetJsonAsync(requestPath)).Deserialize<Register>(_baseClient.serializerOptions);
            return register ?? new Register();
        }

        public async Task<List<TransactionModel>> GetBlueprintTransactions(string registerId)
        {
            var blueprintQuery = "?$filter=MetaData/transactionType eq 'Blueprint'";
            var requestPath = $"{_endpoint}/{registerId}/transactions" + blueprintQuery;
            var response = await _baseClient.GetJsonAsync(requestPath);
            if (response != null)
                return JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;
            return new List<TransactionModel>();
        }

        public async Task<TransactionModel?> GetPublishedBlueprintTransaction(string registerId, string blueprintId)
        {
            // Get latest transaction id


            var blueprintQuery = $"?$filter=MetaData/transactionType eq 'Blueprint' and MetaData/registerId eq '{registerId}' and MetaData/blueprintId eq '{blueprintId}'";
            var requestPath = $"{_endpoint}/{registerId}/transactions" + blueprintQuery;
            var response = await _baseClient.GetJsonAsync(requestPath);
            if (response != null)
            {
                var transactions = JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;

                var mostRecentPublishedTransaction = transactions.FirstOrDefault(t => transactions.All(tr => tr.PrevTxId != t.TxId));
                return mostRecentPublishedTransaction;
            }


            return new TransactionModel();
        }

        public async Task<List<TransactionModel>?> GetParticipantTransactions(string registerId)
        {
            var participantQuery = $"?$filter=MetaData/transactionType eq 'Participant' and MetaData/registerId eq '{registerId}'";
            var requestPath = $"{_endpoint}/{registerId}/transactions" + participantQuery;
            var response = await _baseClient.GetJsonAsync(requestPath);
            if (response != null)
            {
                var transactions = JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;

                return transactions;
            }


            return new List<TransactionModel>();
        }

        public async Task<List<TransactionModel>> GetTransactionsByInstanceId(string registerId, string instanceId)
        {
            var instanceQuery = $"?$filter=MetaData/instanceId eq '{instanceId}'";
            var URI = $"{_endpoint}/{registerId}/transactions" + instanceQuery;
            var response = await _baseClient.GetJsonAsync(URI);
            return JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;
        }

        public async Task<List<TransactionModel>> GetAllTransactionsBySenderAddress(string registerId, string senderAddress)
        {
            var instanceQuery = $"?$filter=SenderWallet eq '{senderAddress}'";
            var URI = $"{_endpoint}/{registerId}/transactions" + instanceQuery;
            var response = await _baseClient.GetJsonAsync(URI);
            return JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;
        }

        public async Task<List<TransactionModel>> GetAllTransactionsByRecipientAddress(string registerId, string recipientAddress)
        {
            var instanceQuery = $"?$filter=RecipientsWallets/any(wallet: wallet eq '{recipientAddress}')";
            var URI = $"{_endpoint}/{registerId}/transactions" + instanceQuery;
            var response = await _baseClient.GetJsonAsync(URI);
            return JsonSerializer.Deserialize<List<TransactionModel>>(response, _baseClient.serializerOptions)!;
        }

        public async Task SubscribeRegister(string registerId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.SendAsync("SubscribeRegister", registerId);
            }
        }

        public async Task UnSubscribeRegister(string registerId)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.SendAsync("UnSubscribeRegister", registerId);
            }
        }

        public async Task StartEvents()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_baseClient.ServiceHost + "registershub", options =>
                {
                    options.AccessTokenProvider = _baseClient.GetBearer!;
                })
                .ConfigureLogging(logging =>
                {
                    // This will set ALL logging to Debug level
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            _hubConnection.On<TransactionConfirmed>(Topics.TransactionValidationCompletedTopicName, transactionConfirmed =>
            {
                _ = TransactionValidationCompleted(transactionConfirmed);
            });
            await _hubConnection.StartAsync();
            ConnectionId = _hubConnection.ConnectionId;
        }

        public string? ConnectionId { get; set; }
        public event TransactionEventHandler? OnTransactionValidationCompleted;

        public async Task TransactionValidationCompleted(TransactionConfirmed transactionConfirmed)
        {
            _baseClient.Log.LogInformation("Transaction validation completed {type} : {id} ", transactionConfirmed.MetaData.TransactionType, transactionConfirmed.TransactionId);
            await OnTransactionValidationCompleted!(transactionConfirmed);
        }
    }
}