using Siccar.Common.ServiceClients;
using Siccar.Platform;
using Siccar.Platform.Tenants.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siccar.Application;

namespace AdminUiTest
{
    public class FallbackServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return new DummyTenantServiceClient();
        }
    }

    public class DummyTenantServiceClient : ITenantServiceClient
    {
        public Task<List<Tenant>> All()
        {
            var list = new List<Tenant>()
            {
                new Tenant()
                {
                    Id = Guid.NewGuid().ToString()
                },
                new Tenant()
                {
                    Id = Guid.NewGuid().ToString()
                },
            };
            return Task.FromResult(list);
        }

        public Task<string> ClientUpdate(string tenantId, string clientId, Client client)
        {
            throw new NotImplementedException();
        }

        public Task<Tenant> GetTenantById(string id)
        {
            return Task.FromResult(new Tenant()
            {
                Id = Guid.NewGuid().ToString()
            });
        }

        public Task<List<Siccar.Common.ServiceClients.Models.Tenant.Client>> ListClients(string tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<Siccar.Common.ServiceClients.Models.Tenant.Client?> Get(string tenantId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task<Siccar.Common.ServiceClients.Models.Tenant.Client> Create(Siccar.Common.ServiceClients.Models.Tenant.Client client)
        {
            throw new NotImplementedException();
        }

        public Task DeleteClient(string tenantId, string clientId)
        {
            throw new NotImplementedException();
        }

        public Task<Siccar.Common.ServiceClients.Models.Tenant.Client> ClientUpdate(Siccar.Common.ServiceClients.Models.Tenant.Client client)
        {
            throw new NotImplementedException();
        }

        public Task<List<Participant>> GetPublishedParticipants(string registerId)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionModel> PublishParticipant(string registerId, string walletAddress, Participant participant)
        {
            throw new NotImplementedException();
        }

        public Task<Tenant> UpdateTenant(string id, Tenant requestData)
        {
            throw new NotImplementedException();
        }

        public Task SetBearerAsync(string bearer = "")
        {
            throw new NotImplementedException();
        }

        public Task<Participant> GetPublishedParticipantById(string registerId, string participantId)
        { 
            throw new NotImplementedException(); 
        }

        public Task<ODataRaw<List<Participant>>> GetPublishedParticipantsOData(string registerId, string query = "")
        {
            throw new NotImplementedException();
        }
    }
}
