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

using Siccar.Application;
using Siccar.Platform;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public interface ITenantServiceClient : ISiccarServiceClient
	{
		public Task<Tenant> GetTenantById(string id);
		public Task<Tenant> UpdateTenant(string id, Tenant requestData);
		public Task<List<Tenant>> All();
		public Task<List<Models.Tenant.Client>> ListClients(string tenantId);
		public Task<Models.Tenant.Client?> Get(string tenantId, string clientId);
		public Task<Models.Tenant.Client> Create(Models.Tenant.Client client);
		public Task DeleteClient(string tenantId, string clientId);
		public Task<Models.Tenant.Client> ClientUpdate(Models.Tenant.Client client);
		public Task<ODataRaw<List<Participant>>> GetPublishedParticipantsOData(string registerId, string query);
		public Task<List<Participant>> GetPublishedParticipants(string registerId);
		public Task<Participant> GetPublishedParticipantById(string registerId, string participantId);
		public Task<TransactionModel> PublishParticipant(string registerId, string walletAddress, Participant participant);
	}
}
