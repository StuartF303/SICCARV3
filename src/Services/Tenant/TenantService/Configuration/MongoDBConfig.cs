using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantService.Configuration
{
    public class MongoDBConfig
    {
        public string Name { get; init; }
        public string Host { get; init; }
        public int Port { get; init; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
