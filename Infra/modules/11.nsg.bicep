@description('Name of the Network Security Group')
param nsgName string 

@description('Azure region for deployment')
param location string 

@description('Common tags applied to resources')
param ptags object 

@description('Security rules configuration')
param securityRules array = [
  {
    name: 'AllowInternetCustom443-3443Inbound'
    description: 'External Mode - Client communication to API Management'
    protocol: 'TCP'
    sourcePortRange: '*'
    sourceAddressPrefix: 'Internet'
    destinationAddressPrefix: 'VirtualNetwork'
    access: 'Allow'
    priority: 100
    direction: 'Inbound'
    destinationPortRanges: ['80', '443']
  }
  {
    name: 'AllowTagCustom3443Inbound'
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
  {
    name: 'AllowTagCustom6390Inbound'
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
  {
    name: 'AllowTagCustom443Inbound'
    description: 'Azure Traffic Manager routing for multi-region deployment'
    protocol: 'TCP'
    sourcePortRange: '*'
    destinationPortRange: '443'
    sourceAddressPrefix: 'AzureTrafficManager'
    destinationAddressPrefix: 'VirtualNetwork'
    access: 'Allow'
    priority: 130
    direction: 'Inbound'
  }
  {
    name: 'AllowTagCustom443Outbound'
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
  {
    name: 'AllowTagCustom1433Outbound'
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
  {
    name: 'AllowTagCustom443OutboundKeyVault'
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
  {
    name: 'AllowTagCustom1886-443OutboundforMonitor'
    description: 'Publish Diagnostics Logs, Metrics, Resource Health, and App Insights'
    protocol: 'TCP'
    sourcePortRange: '*'
    sourceAddressPrefix: 'VirtualNetwork'
    destinationAddressPrefix: 'AzureMonitor'
    access: 'Allow'
    priority: 170
    direction: 'Outbound'
    destinationPortRanges: ['1886', '443']
  }
]

resource nsg 'Microsoft.Network/networkSecurityGroups@2024-03-01' = {
  name: nsgName
  location: location
  tags: ptags
  properties: {
    securityRules: [for rule in securityRules: {
      name: rule.name
      properties: {
        description: rule.description
        protocol: rule.protocol
        sourcePortRange: rule.sourcePortRange
        destinationPortRanges: rule.destinationPortRanges != null ? rule.destinationPortRanges : (rule.destinationPortRange != null ? [rule.destinationPortRange] : [])
        sourceAddressPrefix: rule.sourceAddressPrefix
        destinationAddressPrefix: rule.destinationAddressPrefix
        access: rule.access
        priority: rule.priority
        direction: rule.direction
      }
    }]
  }
}

output nsgId string = nsg.id
