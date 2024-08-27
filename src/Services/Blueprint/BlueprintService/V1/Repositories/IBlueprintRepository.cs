using Siccar.Application;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlueprintService.V1.Repositories
{
    public interface IBlueprintRepository
    {
        public Task<List<Blueprint>> GetAll();
        public Task<Blueprint> GetBlueprint(string id);
        public Task SaveBlueprint(Blueprint tenant);
        public Task UpdateBlueprint(Blueprint tenant);
        public Task DeleteBlueprint(string id);
        public Task<bool> BluerpintExists(string id);

    }
}
