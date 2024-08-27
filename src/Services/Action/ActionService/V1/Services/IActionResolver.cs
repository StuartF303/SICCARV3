using Siccar.Application;
using System.Collections.Generic;
using System.Text.Json;

namespace ActionService.V1.Services
{
    public interface IActionResolver
    {
        public Siccar.Application.Action ResolveNextAction(int nextActionId, Blueprint blueprint);
        public string ResolveParticipantWalletAddress(string targetAction, Blueprint blueprint);
        public bool IsFinalAction(Action currentAction, Blueprint blueprint, JsonDocument dataKvp, out int nextActionId);

    }
}
