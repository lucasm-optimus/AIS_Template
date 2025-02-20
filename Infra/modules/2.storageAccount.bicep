param plocation string
param pstorageAccountName string
param pstorageAccountSku string 
param pblobcontainerName string
param ptags object

resource storageaccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: pstorageAccountName
  location: plocation
  kind: 'StorageV2'
  sku: {
    name: pstorageAccountSku
  }
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
  tags: ptags
}

resource blobservice 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storageaccount
  name: 'default'
}

resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: blobservice
  name: pblobcontainerName
}

output storageAccountId string = storageaccount.id
output storageAccountName string = storageaccount.name
