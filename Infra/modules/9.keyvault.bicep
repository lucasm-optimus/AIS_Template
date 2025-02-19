param plocation string
param pkeyVaultName string
param pkeyVaultSku string
param ptags object


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
    accessPolicies: []
    sku: {
      name: pkeyVaultSku
      family: 'A'
    }
  }
}
