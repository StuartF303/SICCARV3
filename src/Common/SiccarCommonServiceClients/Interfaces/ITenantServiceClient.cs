using Siccar.Application;
using Siccar.Platform;
#nullable enable

namespace Siccar.Common.ServiceClients
{
	public interface ITenantServiceClient : ISiccarServiceClient
	{
		public Task<Tenant> GetTenantById(string id);
		public Task<Tenant> UpdateTenant(string id, Tenant requestData);
		public Task<List<Tenant>> All();
		public Task<List<Models.Tenant.Client>> ListClients(string tenantId);
		public Task<Models.Tenant.Client?> Get(string tenantId, string clientId);
		public Task<Models.Tenant.Client> Create(Models.Tenant.Client client);
		public Task DeleteClient(string tenantId, string clientId);
		public Task<Models.Tenant.Client> ClientUpdate(Models.Tenant.Client client);
		public Task<ODataRaw<List<Participant>>> GetPublishedParticipantsOData(string registerId, string query);
		public Task<List<Participant>> GetPublishedParticipants(string registerId);
		public Task<Participant> GetPublishedParticipantById(string registerId, string participantId);
		public Task<TransactionModel> PublishParticipant(string registerId, string walletAddress, Participant participant);
	}
}
