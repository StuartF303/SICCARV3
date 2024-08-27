@description('Cosmos DB account name')
param accountName string = 'cosmos-${uniqueString(resourceGroup().id)}'

@description('Location for the Cosmos DB account.')
param location string = resourceGroup().location

@description('The name for the Core (SQL) database')
param databaseName string

@description('array of containers and their properties')
param containers array

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: toLower(accountName)
  kind: 'GlobalDocumentDB'
  location: location
  properties: {
    enableFreeTier: false
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }
}

resource cosmosDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
  name: toLower(databaseName)
  parent: cosmosAccount
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 400
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-06-15' = [for c in containers: {
  name: toLower(c.name)
  parent: cosmosDB
  properties:{
    resource:{
      id: c.name
      partitionKey:{
        paths:[
          '/${c.partitionKey}'
        ]
      }
    }
  }
}]


output url string = cosmosAccount.properties.documentEndpoint
output masterKey string = listKeys(cosmosAccount.id, cosmosAccount.apiVersion).primaryMasterKey
