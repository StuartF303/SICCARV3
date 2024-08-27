using Siccar.Application;
using System.Collections.Generic;
using System.Text.Json;

namespace ActionService.V1.Factories
{
    public static class ActionFactory
    {
        public static Action BuildAction(Action actionFromBlueprint, string blueprintId, string previousTxId, SortedList<string,string> trackingData)
        {
            var action = new Action()
            {
                Id = actionFromBlueprint.Id,
                Title = actionFromBlueprint.Title,
                Participants = actionFromBlueprint.Participants,
                Sender = actionFromBlueprint.Sender,
                PreviousTxId = previousTxId,
                Blueprint = blueprintId,
                DataSchemas = actionFromBlueprint.DataSchemas,
                Description = actionFromBlueprint.Description,
                Disclosures = actionFromBlueprint.Disclosures,
                Condition = actionFromBlueprint.Condition,
                Form = actionFromBlueprint.Form,
                PreviousData = JsonDocument.Parse(JsonSerializer.Serialize(trackingData))
            };

            return action;
        }
    }
}
