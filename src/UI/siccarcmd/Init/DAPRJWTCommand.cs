using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Siccar.Common;
using Siccar.Common.ServiceClients;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace siccarcmd.init
{
    public class DAPRJWTCommand : Command
    {
        public PeerServiceClient peerHandler = null; 

        public DAPRJWTCommand(string action, IServiceProvider services) : base(action)
        {
            this.Name = "encode";
            this.Description = "Creates a JWT for DAPR";

            peerHandler = services.GetService<ISiccarServiceClient>() as PeerServiceClient;

            Add(new Argument<string>("secret", description: "The Secret to encode as a Bearer"));

            Handler = CommandHandler.Create<string>(CreateJWT);

        }

        private void CreateJWT(string secret)
        {
            if(secret.Length <16)
            {
                throw new ArgumentException("Secret too short needs atleast 16 characters.");
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 
            var header = new JwtHeader(creds);

            // SecretKey : secret
            var payload = new JwtPayload {
                { "SecretKey", secret }
            };

            var token = new JwtSecurityToken(header, payload);

            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(token);

            Console.WriteLine(tokenString);
        }
    }
}
