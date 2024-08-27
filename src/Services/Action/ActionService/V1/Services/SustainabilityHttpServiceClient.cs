using Microsoft.Extensions.Configuration;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActionService.V1.Services
{
    public class SustainabilityHttpServiceClient : ISustainabilityHttpServiceClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientAdaptor _httpClientAdaptor;

        public SustainabilityHttpServiceClient(IConfiguration configuration, IHttpClientAdaptor httpClientAdaptor)
        {
            _configuration = configuration;
            _httpClientAdaptor = httpClientAdaptor;
        }

        public async Task SendPostRequest(TransactionConfirmed transactionConfirmedPayload)
        {
            var makeRequests = _configuration["MakeSusMgrRequests"] != null ? bool.Parse(_configuration["MakeSusMgrRequests"]) : false;
            var toWalletsIncludesTargetWallet = transactionConfirmedPayload.ToWallets.Contains(_configuration["WalletAddress"]);

            if (makeRequests && toWalletsIncludesTargetWallet)
            {
                var data = new SusConnectorHttpReqModel
                {
                    previousTxId = transactionConfirmedPayload.TransactionId,
                    registerId = transactionConfirmedPayload.MetaData.RegisterId,
                    walletAddress = _configuration["WalletAddress"]
                };

                var content = JsonSerializer.Serialize(data, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                await _httpClientAdaptor.PostAsync(_configuration["SusConnectorHttpEndpoint"], content);
            }

        }

        public class SusConnectorHttpReqModel
        {
            public string previousTxId { get; set; }
            public string walletAddress { get; set; }
            public string registerId { get; set; }
        }
    }
}
