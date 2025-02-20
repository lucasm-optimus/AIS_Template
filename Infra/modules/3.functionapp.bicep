param plocation string
param pfunctionAppName string
param pstorageAccountName string
param pstorageAccountID string
param pAppinsightsId string
param pAppServicePlanName string
param pAppServicePlanSku string
param pAppServicePlanSkutier string
param ptags object

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: pAppServicePlanName
  location: plocation
  sku: {
    name: pAppServicePlanSku
    tier: pAppServicePlanSkutier
  }
  properties: {}
  tags: ptags
}

// Azure Function App
resource azureFunction 'Microsoft.Web/sites@2024-04-01' = {
  name: pfunctionAppName
  location: plocation
  kind: 'functionapp'
  tags: ptags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsDashboard'
          value: 'DefaultEndpointsProtocol=https;AccountName=${pstorageAccountName};AccountKey=${listKeys(pstorageAccountID, '2021-09-01').keys[0].value}'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${pstorageAccountName};AccountKey=${listKeys(pstorageAccountID, '2021-09-01').keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${pstorageAccountName};AccountKey=${listKeys(pstorageAccountID, '2021-09-01').keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(pfunctionAppName) 
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference(pAppinsightsId, '2015-05-01').InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'DOTNET_isolatedVersion'
          value: '8.0'
        }
        {
          name: 'WEBSITE_PLATFORM'
          value: '64bit'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED'
          value: '1'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION' 
          value: '6.9.1'
        }
      ]
      scmType: 'None'
    }
  }
}

// Outputs
output functionAppId string = azureFunction.id
output appServicePlanId string = appServicePlan.id
output functionAppPrincipalId string = azureFunction.identity.principalId
