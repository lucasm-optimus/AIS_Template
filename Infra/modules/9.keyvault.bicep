param plocation string
param pkeyVaultName string
param pkeyVaultSku string
param ptags object
param objectId string

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: pkeyVaultName
  location: plocation
  tags: ptags
  properties: {
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    softDeleteRetentionInDays: 90
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: objectId
        permissions: {
          secrets: [
            'get'
            'list'
            'set'
            'delete'
            'backup'
            'restore'
            'recover'
            'purge'
          ]
        }
      }
    ]
    sku: {
      name: pkeyVaultSku
      family: 'A'
    }
  }
}
