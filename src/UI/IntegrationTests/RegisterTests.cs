using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siccar.Common.ServiceClients;
using SiccarApplicationTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Siccar.IntegrationTests
{
    public class RegisterTests
    {
        RegisterServiceClient _registerServiceClient;
        private TestData testData = new TestData();

        public RegisterTests(IServiceProvider serviceProvider, string bearer)
        {
            _registerServiceClient = (RegisterServiceClient)serviceProvider.GetService<IRegisterServiceClient>();
            if (_registerServiceClient == null)
                throw new Exception("Cannot instanciate service client [RegsiterServiceClient]");
        }

        public TimeSpan Go_Basic()
        {
            var registerStopwatch = new Stopwatch();

            registerStopwatch.Start();

            var register = _registerServiceClient.CreateRegister(
                new Platform.Register()
                {
                    Advertise = false,
                    Name = "temporary test register"
                }).Result;

            registerStopwatch.Stop();

            if (register == null)
                return TimeSpan.Zero;
            Console.WriteLine($"Created new Register : {register.Id}");

            return registerStopwatch.Elapsed;

        }





    }
}
