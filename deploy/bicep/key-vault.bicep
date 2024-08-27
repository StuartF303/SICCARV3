param keyvaultName string = 'kv${uniqueString(resourceGroup().id)}' // must be globally unique
param keyName string 
param tenant string = subscription().tenantId
param servicePrincipalId string
param secrets array = []
param location string = resourceGroup().location
param sku string = 'Standard'
param tags object = {}

param accessPolicies array = [
  // az ad signed-in-user show --query objectId
  // todo - should be a parm as it will be the Az Pipeline connection (underlying service principal and not 'user')  
  {
    tenantId: tenant
    objectId: 'e26fdff5-fede-4700-9614-cd08e75515ab'  //keith test only    
    permissions: {
        keys: [
          'all'
        ]
        secrets: [
          'all'
        ]
        certificates: [
          'all'
        ]
        storage: [
          'all'
        ]
    }
  }
]

param enabledForDeployment bool = true
param enabledForTemplateDeployment bool = true
param enabledForDiskEncryption bool = true
param enableRbacAuthorization bool = false
param enableSoftDelete bool = false
param softDeleteRetentionInDays int = 90

param networkAcls object = {
  ipRules: []
  virtualNetworkRules: []
}

resource keyvault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: keyvaultName
  location: location
  tags: tags
  properties: {
    tenantId: tenant
    sku: {
      family: 'A'
      name: sku
    }
    accessPolicies: accessPolicies
    enabledForDeployment: enabledForDeployment
    enabledForDiskEncryption: enabledForDiskEncryption
    enabledForTemplateDeployment: enabledForTemplateDeployment
    softDeleteRetentionInDays: softDeleteRetentionInDays
    enableRbacAuthorization: enableRbacAuthorization
    enableSoftDelete: enableSoftDelete
    networkAcls: networkAcls
  }
}

// seems under the bonnet ARM uses rbac and not the access policy
// when you deploy resources with bicep, the identity of the userservice principal executing 
// the deployment is NOT exposed to the deployment environment
// role assignments so aks can access the vault for keys
// see https://github.com/Azure/bicep/issues/2031 (8-Nov-2021)
// identity so it can execute the bicep script
var currentUserPrincipalId = 'e26fdff5-fede-4700-9614-cd08e75515ab'
var roleOwner = '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'

resource keyvaultOwnerRoleAssignmentRunningUser 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid(resourceGroup().id, keyvaultName, servicePrincipalId, 'keyvaultOwnerRoleAssignmentRunningUser')
  scope: keyvault
  properties: {
    description: 'Temporary solution to rbc error - grant your object id the owner role'
    principalId: currentUserPrincipalId   //USE YOUR (or a particular) OBJECT ID
    principalType: 'User'   //user here and NOT service principal
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/${roleOwner}'    
  }
}

// create key
// errors on rbac forbidden (without the explicit owner role assignment first)
// could be a bicep bug as it does not happen with secrets
resource key 'Microsoft.KeyVault/vaults/keys@2021-06-01-preview' = if(!empty(keyName)) {
  name: '${keyvault.name}/${keyName}'
  tags: {}
  dependsOn: [
    keyvaultOwnerRoleAssignmentRunningUser
  ]
  properties: {
    kty: 'RSA' // key type
    keyOps: [
      // key operations
      'encrypt'
      'decrypt'
    ]
    keySize: 2048   //4096
  }
}

// create secrets looping thru param
resource secret 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = [for s in secrets: {
  name: '${keyvault.name}/${s.secretName}'
  tags: {}
  properties: {
    value: s.secretValue
  }
}]

var roleKeyVaultSecretsUser = '4633458b-17de-408a-b874-0445c86b69e6'
var roleKeyVaultCryptoOfficer = '14b46e9e-c2b7-41b4-b07b-48a6ebf60603'

// role assignments so aks can access the vault for secrets
resource keyvaultSecretsRoleAssignmentAks 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid(resourceGroup().id, keyvaultName, servicePrincipalId, 'keyvaultSecretsRoleAssignmentAks')
  scope: keyvault
  properties: {
    description: 'Assign key vault secrets user to aks service principal'
    principalId: servicePrincipalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/${roleKeyVaultSecretsUser}'    
  }
}
// role assignments so aks can access the vault for keys 
resource keyvaultKeysRoleAssignmentAks 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid(resourceGroup().id, keyvaultName, servicePrincipalId, 'keyvaultKeysRoleAssignmentAks')
  scope: keyvault
  properties: {
    description: 'Assign key vault keys user to aks service principal'
    principalId: servicePrincipalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/${roleKeyVaultCryptoOfficer}'    
  }
}

// output proxyKey object = key
output keyVaultName string = keyvault.name
output keyVaultId string = keyvault.id
output keyVaultUri string = keyvault.properties.vaultUri

// FYI validate / what-if / create
//
// az deployment group create `      
//    --resource-group Test77 `
//    --template-file ./key-vault.bicep `
//    --parameters kv.parameters.json
//
// FYI testing - get aks principal id
// az aks show -n aks-siccar -g Test77 --query "identity"  
// "7be9ee54-938f-4b1f-88b9-daae923632c9"

 
