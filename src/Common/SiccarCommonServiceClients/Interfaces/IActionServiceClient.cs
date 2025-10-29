// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Platform;
using Siccar.Application;
using Action = Siccar.Application.Action;
#nullable enable

namespace Siccar.Common.ServiceClients
{
		public interface IActionServiceClient : ISiccarServiceClient
		{
				public string? ConnectionId { get; set; }
				public Task StartEvents();
				public Task SetBearer(string Bearer);
				public Task SubscribeWallet(string walletAddress);
				public Task UnSubscribeWallet(string walletAddress);
				public delegate Task ConfirmedMessageHandler(TransactionConfirmed transactionMessage);
				public event ConfirmedMessageHandler OnConfirmed;
				public Task<List<Action>> GetAll(string walletAddress, string registerId);
				public Task<List<Action>> GetStartingActions(string walletAddress, string registerId);
				public Task<Action> GetAction(string walletAddress, string registerId, string txId, string? accessToken = null, bool aggregatePreviousTransactionData = true);
				public Task<TransactionModel> Submission(ActionSubmission submission);
				public Task<TransactionModel> Rejection(ActionSubmission submission);
		}
}
