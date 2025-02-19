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
          value: 'dotnet'
        }
      ]
    }
  }
}

// Outputs
output functionAppId string = azureFunction.id
output appServicePlanId string = appServicePlan.id
output functionAppPrincipalId string = azureFunction.identity.principalId
