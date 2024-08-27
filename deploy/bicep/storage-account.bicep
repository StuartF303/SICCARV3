@minLength(3)
@maxLength(24)
@description('The name of the storage account (lower case letters and numbersonly) - unique across Azure.')
param storageName string
//az configure --defaults group=[dev resource group name]
//input parm should use uniqueString(resourceGroup().id)

resource exampleStorage 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageName
  location: 'UK South'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

//output name
