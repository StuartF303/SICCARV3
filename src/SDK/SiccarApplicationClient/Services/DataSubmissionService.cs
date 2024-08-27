using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Json.Schema;

namespace Siccar.Application.Client.Services
{
    public class DataSubmissionService
    {
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        private Dictionary<string, object> FormData { get; } = new Dictionary<string, object>();
        public bool IsSubmitDisabled { get; private set; } = false;

        public void UpdateFormData(string key, object data)
        {
            FormData[key] = data;

            //NotifyStateChanged();
        }

        public object GetFormDataByKey(string key)
        {
            if (!FormData.ContainsKey(key) )
            {
                return null;
            }
            FormData.TryGetValue(key, out var data);

            return data;
        }

        public JsonDocument GetFormDataAsJson()
        {
            return JsonDocument.Parse(JsonSerializer.Serialize(FormData, serializerOptions));
        }

        public void ClearData()
        {
            FormData.Clear();
        }

        public void SetSubmissionState(bool isSubmitDisabled)
        {
            IsSubmitDisabled = isSubmitDisabled;
            NotifyStateChanged();
        }

        public event System.Action StateChanged;

        private void NotifyStateChanged()
            => StateChanged?.Invoke();
    }
}
