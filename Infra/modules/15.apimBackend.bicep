param apimServiceName string
param backendUrl string
param backendName string
param resourceId string


resource apimService 'Microsoft.ApiManagement/service@2024-05-01' existing = {
  name: apimServiceName
}

resource backend 'Microsoft.ApiManagement/service/backends@2024-05-01' = {
  parent: apimService
  name: backendName
  properties: {
    url: backendUrl
    protocol: 'http'
    resourceId: resourceId
    credentials: {
    }
  }
}
