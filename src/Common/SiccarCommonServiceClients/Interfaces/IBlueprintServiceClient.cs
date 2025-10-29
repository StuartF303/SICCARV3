// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Application;
using Siccar.Platform;

namespace Siccar.Common.ServiceClients
{
	public interface IBlueprintServiceClient
	{
		public Task<Blueprint> GetBlueprintDraft(string blueprintId);
		public Task<Blueprint> CreateBlueprintDraft(Blueprint blueprint);
		public Task<Blueprint> UpdateBlueprintDraft(Blueprint blueprint);
		public Task<TransactionModel> PublishBlueprint(string walletAddress, string registerId, Blueprint blueprint);
		public Task<List<Blueprint>> GetAllPublished(string walletAddress, string registerId);
		public Task<List<Blueprint>> GetAllPublished(string registerId);
		public Task<List<Blueprint>> GetAllDraft();
		public Task<Blueprint> GetPublished(string walletAddress, string registerId, string blueprintId);
		public Task<Blueprint> GetPublished(string registerId, string blueprintId);
	}
}
