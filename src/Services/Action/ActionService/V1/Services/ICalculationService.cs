using Siccar.Application;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActionService.V1.Services
{
    public interface ICalculationService
    {
        public Task<JsonDocument> RunActionCalculationsAsync(Action currentAction, ActionSubmission actionSubmission, string instanceId);
    }
}
