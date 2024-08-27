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

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Application.Client.Models;
using Siccar.Common.ServiceClients;
using Siccar.Application.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Siccar.Common.Adaptors;

namespace Siccar.Application.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSiccarSDKStateManagement(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("siccarClient", s => s.BaseAddress = new Uri(configuration["SiccarService"]));

            var tokenProvider = services.BuildServiceProvider().GetService<IAccessTokenProvider>();

            if (tokenProvider == null)
                services.AddSingleton<IAccessTokenProvider, NullAccessTokenProvider>();

            services.AddHttpContextAccessor();
            var UserAuth = new UserAuthentication(configuration);
            services.AddSingleton<UserAuthentication>(UserAuth);
            services.AddSingleton<ClaimsPrincipal>(UserAuth.principal);
            services.AddSingleton<SiccarBaseClient>();

            var baseServicesProviders = services.BuildServiceProvider();
            var baseServiceProvider = baseServicesProviders.GetRequiredService<SiccarBaseClient>();

            // add our services 
            services.AddTransient<IActionServiceClient, ActionServiceClient>(
                _ => new ActionServiceClient(baseServiceProvider));
            services.AddTransient<IFileServiceClient, FileServiceClient>(
                _ => new FileServiceClient(baseServiceProvider));
            services.AddTransient<IBlueprintServiceClient, BlueprintServiceClient>(
                _ => new BlueprintServiceClient(baseServiceProvider));
            services.AddTransient<IWalletServiceClient, WalletServiceClient>(
                _ => new WalletServiceClient(baseServiceProvider));
            services.AddTransient<IRegisterServiceClient, RegisterServiceClient>(
                _ => new RegisterServiceClient(baseServiceProvider));
            services.AddTransient<ValidatorServiceClient, ValidatorServiceClient>(
                _ => new ValidatorServiceClient(baseServiceProvider));
            services.AddTransient<PeerServiceClient, PeerServiceClient>(
                _ => new PeerServiceClient(baseServiceProvider));
            services.AddTransient<ITenantServiceClient, TenantServiceClient>(
                _ => new TenantServiceClient(baseServiceProvider));
            services.AddTransient<IUserServiceClient, UserServiceClient>(
                _ => new UserServiceClient(baseServiceProvider));

            services.AddSingleton<DataSubmissionService>();
            services.AddSingleton<UserInstanceDataService>();
            services.AddSingleton<HeroDemoServicesLogic>();

            return services;
        }
    }
}
