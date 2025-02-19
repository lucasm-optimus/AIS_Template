// Parameters for flexibility
param location string 
param virtualNetworkName string 
param addressSpacePrefixes array = ['10.254.0.0/24']
param subnetName string = 'APIM'
param subnetAddressPrefixes array = ['10.254.0.0/29']
param networkSecurityGroupId string 

// Create Virtual Network
resource vnet 'Microsoft.Network/virtualNetworks@2024-03-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: addressSpacePrefixes
    }
  }
}

// Create Subnet inside VNet
resource subnet 'Microsoft.Network/virtualNetworks/subnets@2024-03-01' = {
  name: subnetName
  parent: vnet
  properties: {
    addressPrefixes: subnetAddressPrefixes
    networkSecurityGroup: {
      id: networkSecurityGroupId
    }
    privateEndpointNetworkPolicies: 'Disabled'
    privateLinkServiceNetworkPolicies: 'Enabled'
  }
}

// Outputs for reference
output vnetName string = vnet.name
output subnetId string = subnet.id
