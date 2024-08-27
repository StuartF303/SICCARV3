using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using BlueprintService.Exceptions;
using Siccar.Application;
using BlueprintService.Configuration;
using Siccar.Common.Adaptors;

namespace BlueprintService.V1.Repositories
{
    public class DaprBlueprintRepository : IBlueprintRepository
    {
        private readonly ILogger<DaprBlueprintRepository> _logger;
        private readonly IDaprClientAdaptor _client;
        private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public DaprBlueprintRepository(ILogger<DaprBlueprintRepository> logger, IDaprClientAdaptor client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<List<Blueprint>> GetAll()
        {
            var allBlueprintKeys = await _client.GetStateAsync<List<string>>(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey);

            if (allBlueprintKeys is null)
            {
                _logger.LogWarning("Could not get all blueprint keys. No state exists for key: {GetAllBlueprintsKey}", BlueprintConstants.GetAllBlueprintsKey);
                return new List<Blueprint>();
            }

            if (allBlueprintKeys.Count == 0) return new List<Blueprint>();

            var allItems = await _client.GetBulkStateAsync(BlueprintConstants.StoreName, allBlueprintKeys, -1);

            var allBlueprints = new List<Blueprint>();

            foreach (var item in allItems)
            {
                if (item.Value == string.Empty)
                {
                    continue;
                }

                allBlueprints.Add(JsonSerializer.Deserialize<Blueprint>(item.Value, _serializerOptions));
            }

            return allBlueprints;
        }


        public async Task<Blueprint> GetBlueprint(string id)
        {
            var blueprint = await _client.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, id);

            if (blueprint is null)
            {
                _logger.LogWarning("Blueprint not found with id {id}", id);
                throw new DaprBlueprintRepositoryException(
                    HttpStatusCode.NotFound,
                    $"Blueprint with id: {id} does not exist."
                    );
            }
            else
            {
                _logger.LogInformation("Retrieved Blueprint: {BlueprintId}, {Blueprint}", blueprint.Id, blueprint);
                return blueprint;
            }
        }

        public async Task<bool> BluerpintExists(string id)
        {
            var blueprint = await _client.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, id);

            return blueprint != null;
        }

        public async Task SaveBlueprint(Blueprint blueprint)
        {
            var existingBlueprint = await _client.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprint.Id);

            if (existingBlueprint is null)
            {
                var allBlueprintKeys = await GetAllBlueprintKeys(_client);
                allBlueprintKeys.Add(blueprint.Id);
                await _client.SaveStateAsync(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey, allBlueprintKeys);

                await _client.SaveStateAsync(BlueprintConstants.StoreName, blueprint.Id, blueprint);
                _logger.LogInformation("Saved Blueprint. {BlueprintId}, {Blueprint}", blueprint.Id, blueprint);
            }
            else
            {
                _logger.LogWarning("Blueprint was not saved, Blueprint with id: {Id} already exists.", blueprint.Id);
                throw new DaprBlueprintRepositoryException(
                    HttpStatusCode.BadRequest,
                    $"Blueprint was not saved, Blueprint with id: {blueprint.Id} already exists."
                    );
            }
        }

        public async Task UpdateBlueprint(Blueprint blueprint)
        {
            var oldTenant = await _client.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, blueprint.Id);
            if (oldTenant is null)
            {
                _logger.LogWarning("Blueprint could not be updated. Blueprint with id {Id}, does not exist", blueprint.Id);
                throw new DaprBlueprintRepositoryException(
                    HttpStatusCode.NotFound,
                    $"Blueprint could not be updated. Blueprint with id {blueprint.Id}, does not exist"
                    );
            }
            else
            {
                await _client.SaveStateAsync(BlueprintConstants.StoreName, blueprint.Id, blueprint);
                _logger.LogInformation("Blueprint with id: {Id}, updated successfully", blueprint.Id);
            }
        }

        public async Task DeleteBlueprint(string id)
        {
            var blueprint = await _client.GetStateAsync<Blueprint>(BlueprintConstants.StoreName, id);

            if (blueprint is null)
            {
                _logger.LogWarning("Blueprint could not be deleted. Blueprint with id {Id}, does not exist", id);
                throw new DaprBlueprintRepositoryException(
                    HttpStatusCode.NotFound,
                    $"Blueprint could not be deleted. Blueprint with id {id}, does not exist"
                    );
            }
            else
            {
                var allBlueprintKeys = await GetAllBlueprintKeys(_client);

                await _client.DeleteStateAsync(BlueprintConstants.StoreName, id);

                allBlueprintKeys.Remove(id);
                await _client.SaveStateAsync(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey, allBlueprintKeys);

                _logger.LogInformation("Blueprint with id: {Id}, deleted successfully", id);
            }
        }

        private async Task<List<string>> GetAllBlueprintKeys(IDaprClientAdaptor client)
        {
            var allBlueprintKeys = await client.GetStateAsync<List<string>>(BlueprintConstants.StoreName, BlueprintConstants.GetAllBlueprintsKey);
            if (allBlueprintKeys is null)
            {
                _logger.LogWarning("Could not get all blueprint keys. No state exists for key: {GetAllBlueprintsKey}", BlueprintConstants.GetAllBlueprintsKey);
                return new List<string>();
            }

            return allBlueprintKeys;
        }
    }
}
