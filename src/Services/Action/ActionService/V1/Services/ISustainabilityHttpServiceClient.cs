using Siccar.Platform;
using System.Threading.Tasks;

namespace ActionService.V1.Services
{
    public interface ISustainabilityHttpServiceClient
    {
        Task SendPostRequest(TransactionConfirmed transactionConfirmedPayload);
    }
}