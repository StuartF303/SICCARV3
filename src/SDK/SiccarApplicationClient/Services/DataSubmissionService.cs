/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

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
