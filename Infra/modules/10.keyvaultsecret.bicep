param vaultName string 
param secrets array 

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' existing = {
  name: vaultName
}

resource keyVaultSecrets 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = [for secret in secrets: {
  parent: keyVault
  name: secret
  properties: {
    value: ' '  
    attributes: {
      enabled: true
    }
  }
}]
