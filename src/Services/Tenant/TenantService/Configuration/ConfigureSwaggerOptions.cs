using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Siccar.Common;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace TenantService.Configuration
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {

            options.SwaggerDoc("v1", CreateInfoForApiVersion());
        }

        static OpenApiInfo CreateInfoForApiVersion()
        {
            var info = new OpenApiInfo
            {
                Title = "Tenant Service",
                Version = "v1",
                Description = "Creates tenant and handles authentication/authorization, part of Siccar.",
                TermsOfService = new Uri(Constants.TermsOfServiceURI),
                Contact = new OpenApiContact
                {
                    Name = Constants.ContactName,
                    Email = Constants.ContactEmail,
                    Url = new Uri("https://www.siccar.net/"),
                },
                License = new OpenApiLicense
                {
                    Name = Constants.LicenseName,
                    Url = new Uri(Constants.LicenseURI)
                }
            };

            return info;
        }
    }
}
