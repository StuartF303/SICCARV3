// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Siccar.Application;
using Siccar.Platform;
using System.Text.Json;
using Action = Siccar.Application.Action;
#nullable enable

namespace Siccar.Common.ServiceClients
{
    /// <summary>
    /// Action Service extension of a Siccar Base Client 
    /// </summary>
    public class ActionServiceClient : IActionServiceClient
    {
        private readonly SiccarBaseClient _baseClient;
        private readonly string _endpoint = "actions";
        private HubConnection? hubConnection;

        public event IActionServiceClient.ConfirmedMessageHandler? OnConfirmed;
        public string? ConnectionId { get; set; }

        public ActionServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }
        public async Task<List<Action>> GetStartingActions(string walletAddress, string registerId)
        {
            var response = await _baseClient.GetJsonAsync($"{_endpoint}/{walletAddress}/{registerId}/blueprints");
            var responseData = JsonSerializer.Deserialize<List<Action>>(response, _baseClient.serializerOptions);
            return responseData!;
        }
        public async Task<List<Action>> GetAll(string walletAddress, string registerId)
        {
            var response = await _baseClient.GetJsonAsync($"{_endpoint}/{walletAddress}/{registerId}");
            return JsonSerializer.Deserialize<List<Action>>(response, _baseClient.serializerOptions)!;
        }
        public async Task<Action> GetAction(string walletAddress, string registerId, string txId, string? accessToken = null, bool aggregatePreviousTransactionData = true)
        {
            var response = await _baseClient.GetJsonAsync($"{_endpoint}/{walletAddress}/{registerId}/{txId}?aggregatePreviousTransactionData={aggregatePreviousTransactionData}");
            return JsonSerializer.Deserialize<Action>(response, _baseClient.serializerOptions)!;
        }
        public async Task<TransactionModel> Submission(ActionSubmission submission)
        {
            var callPath = $"{_endpoint}";
            var payload = JsonSerializer.Serialize(submission, _baseClient.serializerOptions);
            var response = await _baseClient.PostJsonAsync(callPath, payload);
            return JsonSerializer.Deserialize<TransactionModel>(response, _baseClient.serializerOptions)!;
        }
        public async Task<TransactionModel> Rejection(ActionSubmission submission)
        {
            var callPath = $"{_endpoint}/reject";
            var payload = JsonSerializer.Serialize(submission, _baseClient.serializerOptions);
            var response = await _baseClient.PostJsonAsync(callPath, payload);
            return JsonSerializer.Deserialize<TransactionModel>(response, _baseClient.serializerOptions)!;
        }

        public async Task SetBearer(string Bearer)
        {
            await _baseClient.SetBearerAsync(Bearer);
        }

        public async Task StartEvents()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_baseClient.ServiceHost + "actionshub", options =>
                {
                    options.AccessTokenProvider = _baseClient.GetBearer!;
                })
                .ConfigureLogging(logging =>
                 {
                     // This will set ALL logging to Debug level
                     logging.SetMinimumLevel(LogLevel.Information);
                 })
                .Build();

            hubConnection?.On<TransactionConfirmed>("ReceiveAction", async (metatx) =>
            {
                await ReceiveAction(metatx);
            });
            await Task.Run(() => hubConnection?.StartAsync().Wait());
            ConnectionId = hubConnection?.ConnectionId!;
        }
        public async Task SubscribeWallet(string walletAddress)
        {
            // todo: a whole bunch of checking
            // walletAddress - real, areyou allowed
            // connection state etc...
            await hubConnection!.SendAsync("SubscribeWallet", walletAddress);
        }
        public async Task UnSubscribeWallet(string walletAddress)
        {
            // todo: a whole bunch of checking
            // walletAddress - real, areyou allowed
            // connection state etc...
            await hubConnection?.SendAsync("UnSubscribeWallet", walletAddress)!;
        }

        public delegate Task ConfirmedMessageHandler(TransactionConfirmed transactionMessage);

        // we want to use this as a hook, to fire a local event and some housekeeping
        public async Task ReceiveAction(TransactionConfirmed metatx)
        {
            // lets just have a look for the moment
            _baseClient?.Log.LogInformation("Confirmed {type} : {id} ", metatx.MetaData.TransactionType, metatx.TransactionId);
            await OnConfirmed!(metatx);
        }
        public class SubmitActionResponse
        {
            public string? Id { get; set; }
        }
        public async Task SetBearerAsync(string bearer = "")
        {
            await _baseClient.SetBearerAsync(bearer);
        }
    }
}
