// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using System.Text.Json;
using Siccar.Platform;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public interface IUserServiceClient : ISiccarServiceClient
	{
		public Task<List<User>> All();
		Task<HttpResponseMessage> AddToRole(Guid id, string role);
		Task<HttpResponseMessage> Delete(Guid id);
		Task<JsonDocument> Get(Guid id);
		Task<HttpResponseMessage> RemoveFromRole(Guid id, string role);
	}
}