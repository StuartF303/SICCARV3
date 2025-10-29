// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using Siccar.Application;
using Siccar.Platform;
using System.Text.Json;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public class BlueprintServiceClient : IBlueprintServiceClient
	{
		private readonly SiccarBaseClient _baseClient;
		private readonly string _endpoint = "blueprints";

		public BlueprintServiceClient(SiccarBaseClient baseClient)
		{
			_baseClient = baseClient;
		}
		public async Task<Blueprint> GetBlueprintDraft(string blueprintId)
		{
			var callPath = $"{_endpoint}/{blueprintId}";
			var response = await _baseClient.GetJsonAsync(callPath);
			return JsonSerializer.Deserialize<Blueprint>(response, _baseClient.serializerOptions)!;
		}
		public async Task<Blueprint> CreateBlueprintDraft(Blueprint blueprint)
		{
			var callPath = $"{_endpoint}";
			var payload = JsonSerializer.Serialize(blueprint, _baseClient.serializerOptions);
			var response = await _baseClient.PostJsonAsync(callPath, payload);
			return JsonSerializer.Deserialize<Blueprint>(response, _baseClient.serializerOptions)!;
		}
		public async Task<Blueprint> UpdateBlueprintDraft(Blueprint blueprint)
		{
			var callPath = $"{_endpoint}/{blueprint.Id}";
			var payload = JsonSerializer.Serialize(blueprint, _baseClient.serializerOptions);
			var response = await _baseClient.PutJsonAsync(callPath, payload);
			var contentString = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<Blueprint>(contentString, _baseClient.serializerOptions)!;
		}
		public async Task<TransactionModel> PublishBlueprint(string walletAddress, string registerId, Blueprint blueprint)
		{
			var callPath = $"{_endpoint}/{walletAddress}/{registerId}/publish";
			var payload = JsonSerializer.Serialize(blueprint, _baseClient.serializerOptions);
			var response = await _baseClient.PostJsonAsync(callPath, payload);
			return JsonSerializer.Deserialize<TransactionModel>(response, _baseClient.serializerOptions)!;
		}
		public async Task<List<Blueprint>> GetAllPublished(string walletAddress, string registerId)
		{
			var callPath = $"{_endpoint}/{registerId}/published?wallet-address={walletAddress}";
			var response = await _baseClient.GetJsonAsync(callPath);
			return JsonSerializer.Deserialize<List<Blueprint>>(response, _baseClient.serializerOptions)!;
		}
		public async Task<List<Blueprint>> GetAllPublished(string registerId)
		{
			var callPath = $"{_endpoint}/{registerId}/published";
			var response = await _baseClient.GetJsonAsync(callPath);
			return JsonSerializer.Deserialize<List<Blueprint>>(response, _baseClient.serializerOptions)!;
		}
		public async Task<List<Blueprint>> GetAllDraft()
		{
			var callPath = $"{_endpoint}";
			var response = await _baseClient.GetJsonAsync(callPath);
			return JsonSerializer.Deserialize<List<Blueprint>>(response, _baseClient.serializerOptions)!;
		}
		public async Task<Blueprint> GetPublished(string walletAddress, string registerId, string blueprintId)
		{
			var callPath = $"{_endpoint}/{registerId}/{blueprintId}/published?wallet-address={walletAddress}";
			var response = await _baseClient.GetJsonAsync(callPath);
			return response.Deserialize<Blueprint>(_baseClient.serializerOptions)!;
		}
		public async Task<Blueprint> GetPublished(string registerId, string blueprintId)
		{
			var callPath = $"{_endpoint}/{registerId}/{blueprintId}/published";
			var response = await _baseClient.GetJsonAsync(callPath);
			return response.Deserialize<Blueprint>(_baseClient.serializerOptions)!;
		}
	}
}
