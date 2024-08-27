


// Specifies the Azure location where the vnet should be created.
param location string =resourceGroup().location
// Tag information for vnet
@description('Tags for the vnet')
param tags object = {}
// Virtual network name
@description('The virtual network name')
param virtualNetworkName string
// Address prefix for virtual network
@description('The virtual network address prefixes')
param addressPrefix string = '172.27.0.0/16'
// Subnet prefix for virtual network
@description('The subnet address prefix')
param subnetPrefix string = '172.27.0.0/24'
// Subnet name
@description('The name of the subnet')
param subnetName string

// Azure virtual network
resource vn 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        addressPrefix
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetPrefix
        }
      }
    ]
  }
}

output id string = vn.id
output name string = vn.name
