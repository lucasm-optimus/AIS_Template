param pkeyVaultName string
param psecrets array 

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: pkeyVaultName
}

resource keyVaultSecrets 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = [for secret in psecrets: {
  name: secret.name
  parent: keyVault
  properties: {
    value: secret.value
  }
}]
