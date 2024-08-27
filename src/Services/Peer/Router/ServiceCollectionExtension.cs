using Microsoft.Extensions.DependencyInjection;
using Siccar.Network.Peers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Network.Router
{
    public static class ServiceCollectionExtension
    {
        public static void AddFactory<TService, TImplementation>(this IServiceCollection services)
                where TService : class
                where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>());
            services.AddSingleton<IFactory<TService>, Factory<TService>>();
        }
    }
}
