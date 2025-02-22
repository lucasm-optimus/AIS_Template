param apimServiceName string
param apiName string
param displayName string
param serviceUrl string
param path string
param subscriptionRequired bool
param operations array

resource apimService 'Microsoft.ApiManagement/service@2024-05-01' existing = {
  name: apimServiceName
}

// Create the API
resource api 'Microsoft.ApiManagement/service/apis@2024-05-01' = {
  parent: apimService
  name: apiName
  properties: {
    displayName: displayName
    serviceUrl: serviceUrl
    path: path
    protocols: [
      'https'
    ]
    subscriptionRequired: subscriptionRequired
  }
}

// Create operations for the API
resource apiOperations 'Microsoft.ApiManagement/service/apis/operations@2024-05-01' = [for operation in operations: {
  parent: api
  name: operation.name
  properties: {
    displayName: operation.displayName
    method: operation.method
    urlTemplate: operation.urlTemplate
    responses: [ ]
  }
}]

