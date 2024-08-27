# Introduction

The wallet service is concerned with storing wallets, creating transactions and decrypting payloads.

## Prerequisites

This project relies on dapr secret stores. These stores must be bootstraped from a local secret file. 
You must create a file called `secretsFile.json` and place it in the components folder.

It should contain the following secrets

{
  "walletSecrets": {
    "keyVaultConnectionString": "",
    "siccarV3ClientId": "",
    "siccarV3ClientSecret": "",
    "siccarV3ClientTenant": "",
    "walletEncryptionKey": ""
  }
}

You will also need to create a appsettings.Development.json file and populate with the values from the ms teams SiccarV3 wiki

## Getting started
This project should be run as a part of the Docker Compose project

## Building

WalletDB Migrations:

    When updating the data schmea you may need to migrate the Database. EF Core will automate this but you will need to scaffold the migration as follows:

    in the \src\ServicesWallet\WalletService directory, ensure the appsettings connection string will find an Endpoint and run

    dotnet ef migrations add Initial --project ..\WalletSqlRepository

    This will build the migration, if its early enough we may just delete the initial migration. 