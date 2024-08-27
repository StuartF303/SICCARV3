# Siccar Releasenotes

## 30th March 2023
Added volumes for the mongodb, mysql, rabbitmq and redis containers in docker-compose.override.yml so that data is persisted between cleans or restarts of Visual Studio.

## Kanban 2

The tenant service issuer needs to be updated in the appsettings.Development.json. This will allow the CLI tool to work.

  "IdentityServer": {
    "IssuerName": "https://localhost:8443"
  }
## SQL Wallets

Local environment uses MySql with the Pomelo Entity Framework drivers
Cloud Environment targeting Azure Serverless SQL instance

WalletService Readme has info on creating and using DB Migrations

The WalletService will not decrypt the users private key on a repository get unless the requesting identity is in the owner or in the future a delegate

## v0.85

Adding Entity Framewaork based Wallet Repository


## Kanban 1

The Docker cluster now comes up with an nginx reverse proxy service
access is via http://localhost:8080


## Sprint 12 (So lonely)

To allow the TenantService to startup in Docker-Compose it needs a Cerificate, see ReadMe.md 

To execute in Docker desktop using docker compose, ensuring all .env parameters are set

```
 docker-compose build
 docker-compose up -d
 docker-compose ps
```

## Sprint 10 11/10/21

Additional keys have been added to the local dapr secret store. 

```
{
  "walletSecrets": {
    "keyVaultConnectionString": "",
    "siccarV3ClientId": "",
    "siccarV3ClientSecret": "",
    "walletEncryptionKey": ""
  }
}
```

## Sprint 9 - 1/10/21

A Dapr api key has been added to the WalletService appsettings.json and the appsettings.Development.json
has been removed from source control. The API token contains the dapr secret so you could create your own token 
you just need to make sure the secrets match.

```
  "DaprSecret": <daprsecret>,
```

The Dapr access token has been added to the .env file

```
 APP_API_TOKEN=<apitoken>
```

The api token has the following schema: 
{
  "SecretKey": "somerandomguid"
}

## Sprint 9 - 30/09/21

The dockercompose deployment to docker now requires a local .env file. You must create the .env in root of the solution.
The .env file must include the following:
```
 FEED_ACCESSTOKEN=<token>
```
You should create your own PAT(personal access token) via Azure DevOps and add it to the line above.

## Sprint 6 - 23/08/21

Removed PeerService appsettings.Development.json. This will now need to be added manually.

See below for peer config

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Warning",
      "Microsoft.Extensions.Http": "Information"
    }
  },
  "Peer": {
    "Name": "service.siccar.net",
    "DisableTLS": true,
    "UseInternalIP": true,
    "mDNSEnabled": false
  },
  "Serilog": {
    "MinimumLevel": "Verbose",
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:5341",
          "outputTemplate": "{Timestamp:o} [{Level:u3}][{MachineName}/{Application}/{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "{Timestamp:o} [{Level:u3}][{MachineName}/{Application}/{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

## Sprint 6 - 23/08/21

Added Siccar.Net tenant as an IDP to the tenant service. Requires extra configuration in the Tenant appsettings.Development.json
See wiki for full configuration.

```
  "IdentityProviders": {
    "AzureAd": {
      "Authority": "",
      "ClientId": ""
    }
```


## Sprint 3 - 16/07/21

The Tenant service now requires a local copy of appsettings.Development.json which contains secrets.
The configuration for this can be found in the SiccarV3 wiki. See below for the required configuration.

```
  "IdentityProviders": {
    "AzureB2C": {
      "Authority": "",
      "ClientId": ""
    },
    "InternalTestServer": {
      "Authority": "",
      "ClientId": "",
      "ClientSecret": ""
    }
```

## Sprint 2 - 02/07/21

WalletService encryption key, the service now requires an encryption key to encrypt wallets at rest. 
This is done via the DAPR client. A local secretsFile.json is required for the localsecretstore.yaml file in the top level components folder.

secretsFile.json : 
{
  "walletEncryptionKey": "place-encryption-key-here"
}