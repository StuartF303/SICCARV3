param serviceBusNamespaceName string = 'sb-${uniqueString(resourceGroup().id)}'
param serviceBusQueueName string = 'items'
param location string = resourceGroup().location

@allowed([
  'Basic'
  'Standard'
  'Premium'
])
@description('Tier of the Azure Service Bus')
param skuName string = 'Basic'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: skuName
  }
  properties: {}
}

resource serviceBusQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${serviceBusQueueName}'
  properties: {
    lockDuration: 'PT5M'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    defaultMessageTimeToLive: ((skuName != 'Basic') ?'P10675199DT2H48M5.4775807S' : '')
    deadLetteringOnMessageExpiration: false
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    maxDeliveryCount: 10
    autoDeleteOnIdle: ((skuName != 'Basic') ? 'P10675199DT2H48M5.4775807S' : '')
    enablePartitioning: false
    enableExpress: false
  }
}

// seem to have to construct the primary connection string (crap!)
// bicep function listKeys for Service Bus maps to Namespaces/Authorization Rules - List Keys
var endpoint = '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey'
var serviceBusConnectionString = 'Endpoint=sb://${serviceBusNamespace.name}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=${listKeys(endpoint, serviceBusNamespace.apiVersion).primaryKey}'

output serviceBusConnectionString string = serviceBusConnectionString 
