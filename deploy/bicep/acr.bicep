@minLength(5)
@maxLength(50)
@description('Name of the azure container registry (must be globally unique)')
param acrName string = 'acr${uniqueString(resourceGroup().id)}'

@description('The principal ID of the AKS cluster')
param aksPrincipalId string = ''

@description('Enable an admin user that has push/pull permission to the registry.')
param acrAdminUserEnabled bool = false

@description('Location for all resources.')
param location string = resourceGroup().location

@allowed([
  'Basic'
  'Standard'
  'Premium'
])
@description('Tier of the Azure Container Registry.')
param acrSku string = 'Basic'

@description('Tags for the resources')
param tags object 

@allowed([
  'b24988ac-6180-42a0-ab88-20f7382dd24c' // Contributor
  'acdd72a7-3385-48ef-bd42-f606fba81ae7' // Reader
])
param roleAcrPull string = 'b24988ac-6180-42a0-ab88-20f7382dd24c'

// azure container registry
resource acr 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: acrName
  location: location
  tags: tags
  sku: {
    name: acrSku
  }
  properties: {
    adminUserEnabled: acrAdminUserEnabled
  }
  
}

resource assignAcrPullToAks 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = if(!empty(aksPrincipalId)) {
  name: guid(resourceGroup().id, acrName, aksPrincipalId, 'AssignAcrPullToAks')
  scope: acr
  properties: {
    // condition: 'string'
    // conditionVersion: 'string'
    // delegatedManagedIdentityResourceId: 'string'
    description: 'Assign AcrPull role to AKS'
    principalId: aksPrincipalId
    principalType: 'ServicePrincipal'
    // roleDefinitionId: roleAcrPull
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/${roleAcrPull}'    
  }
}

output acrLoginServer string = acr.properties.loginServer

// az deployment group what-if `
//   --resource-group Test77 `
//   --template-file ./acr.bicep `
//   --parameters ./acr.parameters.json
