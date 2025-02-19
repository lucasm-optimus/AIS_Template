@description('Name of the Public IP Address')
param publicIPName string 

@description('Azure region for deployment')
param plocation string 

@description('IP address allocation method')
param ipAllocationMethod string = 'Static'


@description('Domain Name Label for the Public IP')
param pipdomainNameLabel string 

@description('Idle timeout in minutes')
param idleTimeoutInMinutes int = 4

@description('Availability Zones for Public IP')
param zones array = [
  '1'
  '2'
  '3'
]

@description('Common tags applied to resources')
param ptags object 

resource publicIP 'Microsoft.Network/publicIPAddresses@2024-03-01' = {
  name: publicIPName
  location: plocation
  tags: ptags
  sku: {
    name: 'Standard'
    tier: 'Regional'
  }
  zones: zones
  properties: {
    publicIPAddressVersion: 'IPv4'
    publicIPAllocationMethod: ipAllocationMethod
    idleTimeoutInMinutes: idleTimeoutInMinutes
    dnsSettings: {
      domainNameLabel: pipdomainNameLabel
    }
    ipTags: []
    ddosSettings: {
      protectionMode: 'VirtualNetworkInherited'
    }
  }
}

output publicIPName string = publicIP.name
output publicIPId string = publicIP.id
