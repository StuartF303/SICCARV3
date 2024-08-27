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
