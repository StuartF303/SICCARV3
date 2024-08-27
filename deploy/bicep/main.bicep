// Manifest
//
//

// Common attributs
var location = resourceGroup().location
var tags = {
  environment: 'dev'
}
var serviceName = 'siccar'
param env string = 'dev'

// kubernetes attributes
var aksName = 'aks-${serviceName}'
var agentMinCount = 3
var agentMaxCount = 3
@description('Enable Log analytics workspace')
param enableWorkspace bool = true
@description('Enable http application routing (use for dev only)')
param enableHttpApplicationRouting bool = true

// container registry
param acrName string = 'acr${uniqueString(resourceGroup().id)}'
param acrSku string = 'Basic'

//cosmos sql
param cosmosSqlAccountName string = 'cosmos-sql-ac-${env}'
param cosmosSqlDatabaseName string = 'cosmos-sql-db-${env}'

//cosmos mongo
param cosmosMongoAccountName string = 'cosmos-mongo-ac-${env}'
param cosmosMongoDatabaseName string = 'cosmos-mongo-db-${env}'

param serviceBusNamespaceName string = 'sb-${uniqueString(resourceGroup().id)}-${env}'
param serviceBusQueueName string = 'transactions-${env}'
param sbSku string= 'Basic'

//workspace for aks
module workspace './log-analytics.bicep'  = if(enableWorkspace) {
  name: 'workspaceDeploy'
  params: {
    location: location
    workspaceNamePrefix: serviceName
    tags: tags
  }
}

//Kubernetes 
module aks './aks.bicep' = {
  name: 'aksDeploy'
  params: {
    aksName: aksName
    kubernetesVersion: '1.20.9'
    availabilityZones: [
      '1'
      '2'
      '3'
    ]
    agentMinCount: agentMinCount
    agentMaxCount: agentMaxCount
    subnetRef: ''
    workspaceId: enableWorkspace == true ? workspace.outputs.id : ''
    enableHttpApplicationRouting: enableHttpApplicationRouting
    tags: tags
  }
}

//Aure Container Register (ACR)
module acr './acr.bicep' = {
  name: 'acrDeploy'
  params: {
    acrName: acrName
    roleAcrPull: 'acdd72a7-3385-48ef-bd42-f606fba81ae7'
    acrAdminUserEnabled: true
    aksPrincipalId: aks.outputs.principalId
    location: location
    acrSku: acrSku
    tags: tags
  }
}

// Cosmos DB Container list info to be looped over
var cosmosSqlContainers = [
  {
    name: 'blueprintstate'
    partitionKey: 'id'
  }
  {
    name: 'tenantstate'
    partitionKey: 'id'
  }
  {
    name: 'validatorstate'
    partitionKey: 'id'
  }
  {
    name: 'walletstate'
    partitionKey: 'id'
  }
]

//Cosmos DB (sql api)
module cosmosdbsql './cosmosdb-sql.bicep' = {
  name: 'cosmosDBSqlDeploy' 
  params: {
    databaseName: cosmosSqlDatabaseName
    accountName: cosmosSqlAccountName
    containers: cosmosSqlContainers
    location: location
    //other parms todo
  }
}

//todo declare mongo container properties as above in an array - but don't know the indices yet
var cosmosMongoContainerName = 'RegisterState'

// Cosmos DB (mongo api)
module cosmosdbmongo './cosmosdb-mongo.bicep' = {
  name: 'cosmosDBMongoDeploy'
  params: {
    accountName: cosmosMongoAccountName
    collection1Name: cosmosMongoContainerName
    // collection2Name: 
    databaseName: cosmosMongoDatabaseName
    primaryRegion: location
    // secondaryRegion: 
  }
}

// Service Bus
module serviceBus 'service-bus-queue.bicep' = {
  name: 'serviceBusDeploy'
  params: {
    serviceBusNamespaceName: serviceBusNamespaceName
    serviceBusQueueName: serviceBusQueueName
    skuName: sbSku
  }
}


// we don't want to store in source code and also not want to be exposing them from the pipeline logs.
// todo
// https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/key-vault-parameter?tabs=azure-cli

@description('name of the key')
param kvKey string = 'testkey'

@description('secrets array to pass')
param kvSecrets array = [
  {    
    secretName : 'wallet1'
    secretValue  : 'foo'
  }
  // {    
  //   secretName : 'wallet2'
  //   secretValue  : 'bar'
  // }
]

module keyvault './key-vault.bicep' = {
  name: 'keyvaultDeploy'
  params: {
    keyvaultName: 'kv${uniqueString(resourceGroup().id)}'
    servicePrincipalId: aks.outputs.identity
    keyName: kvKey
    secrets: kvSecrets
    tags: {}
  }
}

output workspaceId string = workspace.outputs.id
output workspaceName string = workspace.outputs.name
output aksId string = aks.outputs.id
output aksClusterName string = aks.outputs.name
output aksServerAddress string = aks.outputs.apiServerAddress
output dnsHost string = aks.outputs.dnsHost
output acrServer string = acr.outputs.acrLoginServer
output cosmosUrl string = cosmosdbsql.outputs.url
output comsosMasterKey string = cosmosdbsql.outputs.masterKey
output mongoDatabaseName string = cosmosdbmongo.outputs.mongoDatabaseName
output mongoConnectionString string = cosmosdbmongo.outputs.mongoConnectionString
output keyvaulUri string = keyvault.outputs.keyVaultUri
output serviceBusConnectionString string= serviceBus.outputs.serviceBusConnectionString

//az deployment group create --resource-group Test77 --template-file .\main.bicep  
