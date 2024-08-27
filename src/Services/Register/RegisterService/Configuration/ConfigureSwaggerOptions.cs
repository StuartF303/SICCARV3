using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Siccar.Common;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
#nullable enable

namespace Siccar.Registers.RegisterService.Configuration
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
                Title = "Register Service",
                Version = "v1",
                Description = "A Multi-Register API Service, part of Siccar.",
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
