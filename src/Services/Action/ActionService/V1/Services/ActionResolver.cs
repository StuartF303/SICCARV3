using ActionService.Exceptions;
using Siccar.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Action = Siccar.Application.Action;

namespace ActionService.V1.Services
{
    public class ActionResolver : IActionResolver
    {
        public string ResolveParticipantWalletAddress(string participantId, Blueprint blueprint)
        {
            var nextParticipant = blueprint.Participants.Find(p => p.Id == participantId);
            var nextParticipantWalletAddress = nextParticipant.WalletAddress ?? throw new ActionResolverException($"Participant with Id: {participantId} could not be found in the blueprint.");

            return nextParticipantWalletAddress;
        }

        public Siccar.Application.Action ResolveNextAction(int nextActionId, Blueprint blueprint)
        {
            var nextAction = blueprint.Actions.Find(action => action.Id == nextActionId);
            nextAction = nextAction ?? throw new ActionResolverException($"Action with Id: {nextActionId} could not be found in the blueprint.");

            return nextAction;
        }

        public bool IsFinalAction(Action currentAction, Blueprint blueprint, JsonDocument dataKvp, out int nextActionId)
        {
            nextActionId = ResolveNextActionId(currentAction, blueprint, dataKvp);

            var currentActionIsFinal = nextActionId < 0;
            return currentActionIsFinal;
        }

        private int ResolveNextActionId(Action currentAction, Blueprint blueprint, JsonDocument dataKvp)
        {
            var nextActionId = currentAction.Id + 1; // the default is current++

            if (currentAction.Condition != null) // but its better to be set by condtion
            {
                //data field ids must be one word and not have "." or "/" in the name.
                var ruleStr = currentAction.Condition.ToString();
                Json.Logic.Rule lr = JsonSerializer.Deserialize<Json.Logic.Rule>(ruleStr);
                JsonNode data = JsonNode.Parse(dataKvp.RootElement.GetRawText());
                var result = lr.Apply(data);

                // if the condition is true then actually we mean just move to the next, 
                if (result.AsValue().TryGetValue<int>(out var resultValue))
                {
                    nextActionId = resultValue;
                }
            }

            if (nextActionId > blueprint.Actions.Count) // but nothing matters if its greater than what we have
                nextActionId = -1;

            return nextActionId;
        }

    }
}
