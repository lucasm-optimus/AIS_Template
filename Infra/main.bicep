@description('Resource plocation')
param location string = resourceGroup().location

@description('App Service Plan Name')
param pAppServicePlanName string 

@description('Function App Name')
param pfunctionAppName string 

@description('Storage Account Name')
param pstorageAccountName string 

@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GZRS'
  'Standard_RAGZRS'
])
@description('Storage Account SKU')
param pstorageAccountSku string 

@description('Storage Account SKU tier') 
param pAppServicePlanSkutier string

@description('App Insights Name')
param pappInsightsName string 

@description('Blob Container Name')
param pblobcontainerName string 


@description('App Service Plan SKU')
param pAppServicePlanSku string 

@description('Key Vault Name')
param pkeyVaultName string 

@allowed([
  'standard'
  'premium'
])
@description('Key Vault SKU')
param pkeyVaultSku string 

@description('Service Bus Namespace Name')
param pserviceBusNamespaceName string 

@allowed([
  'Basic'
  'Standard'
  'Premium'
])
@description('Service Bus Namespace SKU Name')
param pserviceBusNamespaceskuName string 

@allowed([
  'Basic'
  'Standard'
  'Premium'
])
@description('Service Bus Namespace SKU Tier')
param pserviceBusNamespaceskuTier string 

@description('Service Bus Namespace SKU Capacity')
param pserviceBusNamespaceskuCapacity int


@description('Enable Partitioning in Service Bus Topic')
param penablePartitioning bool 

@description('Tags for all resources')
param ptags object 

@description('The email address of the owner of the service')
param pAPIMpublisherEmail string 

@description('The name of the owner of the service')
param pAPIMpublisherName string

@description('The name of the API Management service instance')
param papiManagementServiceName string

@allowed([
  'Consumption'
  'Developer'
  'Basic'
  'Basicv2'
  'Standard'
  'Standardv2'
  'Premium'
])
@description('The pricing tier of this API Management service')
param pAPIMsku string

@description('The name of the Log Analytics workspace')
param pworkspaceName string

@description('The name of the topicsAndSubscriptions workspace')
param topicsAndSubscriptions array


targetScope = 'resourceGroup'

module apimModule './modules/1.apim.bicep' = {
  name: 'apimName'
  params: {
    papiManagementServiceName: papiManagementServiceName
    pAPIMpublisherEmail: pAPIMpublisherEmail
    pAPIMpublisherName: pAPIMpublisherName
    ptags: ptags
    pAPIMsku: pAPIMsku
  }
  dependsOn: [
    logAnalyticsModule
  ]
}

module storageAccountModule './modules/2.StorageAccount.bicep' = {
  name: 'storageAccount'
  params: {
    plocation: location
    pstorageAccountName: pstorageAccountName
    pstorageAccountSku: pstorageAccountSku
    pblobcontainerName: pblobcontainerName
    ptags: ptags
  }
}

module functionAppModule './modules/3.functionapp.bicep' = {
  name: 'functionApp'
  params: {
    pfunctionAppName: pfunctionAppName
    pstorageAccountID: storageAccountModule.outputs.storageAccountId
    pstorageAccountName: storageAccountModule.outputs.storageAccountName
    plocation: location
    pAppinsightsId: insightsModule.outputs.oAppInsightsId
    pAppServicePlanName: pAppServicePlanName
    pAppServicePlanSku: pAppServicePlanSku
    pAppServicePlanSkutier: pAppServicePlanSkutier
    ptags: ptags
  }
}

module insightsModule './modules/4.AppInsights.bicep' = {
  name: 'appinsights'
  params: {
    pappInsightsName: pappInsightsName
    plocation: location
    ptags: ptags
  }
}


module serviceBusModule './modules/5.servicebusNameapaces.bicep' = {
  name: 'serviceBus'
  params:{
    plocation: location
    pserviceBusNamespaceName: pserviceBusNamespaceName
    pserviceBusNamespaceskuCapacity: pserviceBusNamespaceskuCapacity
    pserviceBusNamespaceskuName: pserviceBusNamespaceskuName
    pserviceBusNamespaceskuTier: pserviceBusNamespaceskuTier
    ptags: {}
  }
}

module serviceBusTopicsModule './modules/6.ServicebusTopics.bicep' = {
  name: 'serviceBusTopics'
  params: {
    penablePartitioning: penablePartitioning
    pserviceBusNamespaceName: pserviceBusNamespaceName
    topicsAndSubscriptions: topicsAndSubscriptions
  }
  dependsOn: [
    serviceBusModule
  ]
}

module keyVaultModule './modules/9.keyvault.bicep' = {
  name: 'keyVault'
  params: {
    pkeyVaultName: pkeyVaultName
    plocation: location
    pkeyVaultSku: pkeyVaultSku
    ptags: ptags
  }
}

module logAnalyticsModule './modules/8.LogAnalyticsworkspace.bicep' = {
  name: 'logAnalytics'
  params:{
    plocation: location
    ptags: ptags
    pworkspaceName: pworkspaceName
  }
}
