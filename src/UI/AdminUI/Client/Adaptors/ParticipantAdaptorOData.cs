using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Threading.Tasks;
using Siccar.Common.ServiceClients;
using System.Collections.Generic;
using System.Linq;
using Siccar.UI.Admin.Models;
using System;
using Microsoft.IdentityModel.Tokens;

namespace Siccar.UI.Admin.Adaptors
{
    public class ParticipantAdaptorOData : DataAdaptor
    {
        private readonly ITenantServiceClient _tenantServiceClient;

        public ParticipantAdaptorOData()
        {
        }

        public ParticipantAdaptorOData(ITenantServiceClient tenantServiceClient)
        {
            _tenantServiceClient = tenantServiceClient;
        }

        public async override Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            try
            {
                var req = dataManagerRequest.Params;
                var fictRes = new DataResult()
                {
                    Result = new List<ParticipantModel>(),
                    Count = 0
                };

                if (req.IsNullOrEmpty() || req!.Count == 0 || !req!.ContainsKey("RegisterId"))
                {
                    return fictRes;
                }
                if (!req.TryGetValue("RegisterId", out object registerId))
                {
                };
                if (registerId.ToString() == "noregister")
                {
                    return fictRes;
                }

                var sort = dataManagerRequest.Sorted.First();
                var direction = sort.Direction == "ascending" ? "asc" : "desc";
                string _flt = "";
                if (dataManagerRequest.Where != null)
                {
                    var _where = dataManagerRequest.Where.First();
                    var nx = _where.predicates.First();
                    _flt = $"&$Filter={nx.Operator}(tolower({nx.Field}),'{nx.value.ToString().ToLower()}')";
                }

                var resultOData =
                    await _tenantServiceClient.GetPublishedParticipantsOData(registerId.ToString(),
                        $"?$orderby={sort.Name} {direction}  & skip={dataManagerRequest.Skip}&top={dataManagerRequest.Take} &$count=true "+_flt);

                var participantViewModel = resultOData.Value.Select(participant =>
                {
                    var newParticipant = new ParticipantModel
                    {
                        Id = participant.Id,
                        Name = participant.Name,
                        Organisation = participant.Organisation,
                        WalletAddress= participant.WalletAddress,
                        didUri = participant.didUri,
                        useStealthAddress= participant.useStealthAddress
                    };
                    return newParticipant;
                })
                .ToList();

                if (participantViewModel.Any())
                {
                    participantViewModel[0].IsFirstRow = true;
                }

                DataResult dataResult = new()
                {
                    Result = participantViewModel,
                    Count = resultOData.MetaCount
                };

                return dataResult;
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
