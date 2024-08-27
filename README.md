```
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
```

# Introduction

TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project.

## Prerequisites

Within Visual Studio clone the SICCAR V3 repository:

    'File'
    >  'Clone Repository...'
    >  'Browse a Repository'
    >  'Azure DevOps'
    >  'Connect to a Project'
    >  Scroll to find 'SICCAR V3' project and
    >  'Connect'

Set up SICCAR NuGet package feed:

    'Tools'
    >   'NuGet Package Manager'
    >   'Package Manager Settings'
    >   'Package Source'
    >   '+'
    >   Enter the NuGet package link in 'source' [https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/nuget/v3/index.json]


Please see the SiccarV3 wiki

[https://teams.microsoft.com/l/entity/com.microsoft.teamspace.tab.wiki/tab::9db6783e-98c2-4611-88d2-b9165a2fa870?context=%7B%22subEntityId%22%3A%22%7B%5C%22pageId%5C%22%3A90%2C%5C%22origin%5C%22%3A2%7D%22%2C%22channelId%22%3A%2219%3A218c845494794b19ab588dbf4af07064%40thread.skype%22%7D&tenantId=a2b9ca5b-54e5-437e-866e-bd48bfa6159a]

in teams for configuration and values.

You can find copies of the config files for all Services here:

https://teams.microsoft.com/_#/files/Siccar?threadId=19%3Ab8a7537840be4748b351273a91babeab%40thread.skype&ctx=channel&context=Configuration&rootfolder=%252Fsites%252Fdevelopment%252FShared%2520Documents%252FSiccar%252FConfiguration

Config files (copy these to the relevant folders in your local solution directory):

    - PeerService   -- appsettings.Development.json     /src/Services/Peer/PeerService/
    - TenantService -- appsettings.Development.json     /src/Services/Tenant/TenantService/
                    -- default.pfx                      /src/Services/Tenant/TenantService/
    - WalletService -- appsettings.Development.json     /src/Services/Wallet/WalletService/
    - .env                                              /
    - secretsFile.json                                  /components/

# Getting Started
Note: Volumes are attached for the mongodb, mysql, rabbitmq and redis containers. This means that data in these services is persisted when you clean the solution or close Visual Studio.
However, if you are working on schema changes locally you will want to remove the volumes between trying things out.  This can be done by deleting the folders in ~/.docker-conf.

In order to run this locally you will be required to

- Install Docker Desktop 
- Select Docker Compose as the startup project
- Run the Docker Compose project (install WSL2 when prompted)

For any issues running locally this project, you can find a guide [here](./TROUBLESHOOTING.md) for further information.

This project relies on dapr secret stores. These stores must be bootstraped from a local secret file. 
You must create a file called `secretsFile.json` and place it in the components folder.

#### It should contain the following secrets

{
  "keyVaultConnectionString": "",
  "siccarV3ClientId": "",
  "siccarV3ClientSecret": "",
  "walletEncryptionKey": ""
}

#### For kubernetes secret add as follows:

`kubectl create secret generic local-secret-store`

--from-literal=keyVaultConnectionString="`******`"
--from-literal=siccarV3ClientId="`******`"
--from-literal=siccarV3ClientSecret="`******`"
--from-literal=walletEncryptionKey="`******`"

!! You may need to apply addtional access properties to the Siccar platform service account. Requires list as well as read.
apply this... 

```apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: secret-reader
  namespace: default
rules:
- apiGroups: [""] # "" indicates the core API group
  resources: ["secrets"]
  verbs: ["get", "list", "watch"]

# Build and Test

Run tests via `dotnet test`

# Authentication
When testing the endpoints manually it is possible to generate an access token via the postman scripts located in the solution.
Under Authoization in the Collection folder, you can add the scope for the api resource that you want to access.

# Contribute

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:

- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)

# Troubleshooting

    ~/.nuget\plugins\netfx\CredentialProvider.Microsoft\CredentialProvider.Microsoft.exe -I -V Verbose -U "https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/nuget/v3/index.json" 


# Appendix

## Personal Access Token 'PAT'

To build you need access to the private Nuget Feed (https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/) for this you will need a FEED_ACCESSTOKEN in your environment (.env). 
For our Docker-Compose build you should use the .env file in the solution root to store the Token - this is excluded from source control as its personal to you.

**`!! WARNING !!`**
Personal Access Tokens expire, if your build fails because it cant access the nuget feed

To get a personal Access Token, follow the istructions here: https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows
