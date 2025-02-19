@description('Name of the Network Security Group')
param nsgName string 

@description('Azure region for deployment')
param location string 

@description('Common tags applied to resources')
param ptags object 

resource nsg 'Microsoft.Network/networkSecurityGroups@2024-03-01' = {
  name: nsgName
  location: location
  tags: ptags
  properties: {
    securityRules: [
      {
        name: 'AllowInternetCustom443Inbound'
        properties: {
          description: 'External Mode - Client communication to API Management'
          protocol: 'TCP'
          sourcePortRange: '*'
          sourceAddressPrefix: 'Internet'
          destinationAddressPrefix: 'VirtualNetwork'
          access: 'Allow'
          priority: 100
          direction: 'Inbound'
          destinationPortRange: '443'
        }
      }
      {
        name: 'AllowInternetCustom80Inbound'
        properties: {
          description: 'Allow HTTP traffic'
          protocol: 'TCP'
          sourcePortRange: '*'
          sourceAddressPrefix: 'Internet'
          destinationAddressPrefix: 'VirtualNetwork'
          access: 'Allow'
          priority: 105
          direction: 'Inbound'
          destinationPortRange: '80'
        }
      }
      {
        name: 'AllowTagCustom3443Inbound'
        properties: {
          description: 'Management endpoint for Azure portal and PowerShell'
          protocol: 'TCP'
          sourcePortRange: '*'
          destinationPortRange: '3443'
          sourceAddressPrefix: 'ApiManagement'
          destinationAddressPrefix: 'VirtualNetwork'
          access: 'Allow'
          priority: 110
          direction: 'Inbound'
        }
      }
      {
        name: 'AllowTagCustom6390Inbound'
        properties: {
          description: 'Azure Infrastructure Load Balancer'
          protocol: 'TCP'
          sourcePortRange: '*'
          destinationPortRange: '6390'
          sourceAddressPrefix: 'AzureLoadBalancer'
          destinationAddressPrefix: 'VirtualNetwork'
          access: 'Allow'
          priority: 120
          direction: 'Inbound'
        }
      }
      {
        name: 'AllowTagCustom443Outbound'
        properties: {
          description: 'Dependency on Azure Storage for core service functionality'
          protocol: 'TCP'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: 'Storage'
          access: 'Allow'
          priority: 140
          direction: 'Outbound'
        }
      }
      {
        name: 'AllowTagCustom1433Outbound'
        properties: {
          description: 'Access to Azure SQL endpoints for core service functionality'
          protocol: 'TCP'
          sourcePortRange: '*'
          destinationPortRange: '1433'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: 'Sql'
          access: 'Allow'
          priority: 150
          direction: 'Outbound'
        }
      }
      {
        name: 'AllowTagCustom443OutboundKeyVault'
        properties: {
          description: 'Access to Azure Key Vault for core service functionality'
          protocol: 'TCP'
          sourcePortRange: '*'
          destinationPortRange: '443'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: 'AzureKeyVault'
          access: 'Allow'
          priority: 160
          direction: 'Outbound'
        }
      }
      {
        name: 'AllowTagCustom1886Outbound'
        properties: {
          description: 'Publish Diagnostics Logs and Metrics'
          protocol: 'TCP'
          sourcePortRange: '*'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: 'AzureMonitor'
          access: 'Allow'
          priority: 170
          direction: 'Outbound'
          destinationPortRange: '1886'
        }
      }
    ]
  }
}

output nsgId string = nsg.id
