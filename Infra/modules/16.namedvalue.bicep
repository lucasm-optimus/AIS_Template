param functionAppName string
param apiManagementServiceName string

resource azureFunction 'Microsoft.Web/sites@2024-04-01' existing = {
  name: functionAppName
}

var functionHostKey = listkeys('${azureFunction.id}/host/default', azureFunction.apiVersion).functionKeys.default

resource apiManagementService 'Microsoft.ApiManagement/service@2024-06-01-preview' existing = {
  name: apiManagementServiceName
}


@description('Generate named value to store function key')
  resource azApimFunctionAppNamedValueDeployment 'Microsoft.ApiManagement/service/namedValues@2024-06-01-preview' = {
    parent: apiManagementService
    name: '${apiManagementServiceName}-${azureFunction.name}-key'
    properties: {
      displayName: '${azureFunction.name}-key'
      value: functionHostKey
      secret: true
      tags: [
        'key'
        'function'
        'auto'
      ]
    }
  }

  @description('Register the backend for the Webhooks function')
  resource webhooksFunctionBackend 'Microsoft.ApiManagement/service/backends@2024-06-01-preview' = {
    parent: apiManagementService
    name: '${apiManagementServiceName}-${azureFunction.name}-backend'
    properties: {
      protocol: 'http'
      description: 'Function App'
      url: 'https://${azureFunction.properties.defaultHostName}'
      resourceId: uri(environment().resourceManager, azureFunction.id)
      credentials: {
        header: {
          'x-functions-key': [
            '{{${azApimFunctionAppNamedValueDeployment.name}}}'
          ]
        }
     }
    }
  }
