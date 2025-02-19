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

@description('The name of the secrets workspace')
param psecrets array

@description('Name of the Public IP Address')
param publicIPName string 

@description('Domain Name Label for the Public IP')
param pipdomainNameLabel string 

@description('Name of the Network Security Group')
param nsgName string 

@description('Name of the Virtual Network')
param pvirtualNetworkName string


targetScope = 'resourceGroup'

module apimModule './modules/1.apim.bicep' = {
  name: 'apimName'
  params: {
    papiManagementServiceName: papiManagementServiceName
    pAPIMpublisherEmail: pAPIMpublisherEmail
    pAPIMpublisherName: pAPIMpublisherName
    ptags: ptags
    pAPIMsku: pAPIMsku
    publicIpAddressId: publicIPModule.outputs.publicIPId
    subnetResourceId: vnetModule.outputs.subnetId
    plocation: location
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


module serviceBusModule './modules/5.servicebus.bicep' = {
  name: 'serviceBus'
  params:{
    plocation: location
    pserviceBusNamespaceName: pserviceBusNamespaceName
    pserviceBusNamespaceskuCapacity: pserviceBusNamespaceskuCapacity
    pserviceBusNamespaceskuName: pserviceBusNamespaceskuName
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

module logAnalyticsModule './modules/8.LogAnalyticsworkspace.bicep' = {
  name: 'logAnalytics'
  params:{
    plocation: location
    ptags: ptags
    pworkspaceName: pworkspaceName
  }
}

module keyVaultModule './modules/9.keyvault.bicep' = {
  name: 'keyVault'
  params: {
    pkeyVaultName: pkeyVaultName
    plocation: location
    pkeyVaultSku: pkeyVaultSku
    ptags: ptags
    objectId: functionAppModule.outputs.functionAppPrincipalId
  }
}

module keyVaultSecretModule './modules/10.keyvaultsecret.bicep' = {
  name: 'keyVaultSecret'
  params: {
    secrets: psecrets
    vaultName: pkeyVaultName
  }
  dependsOn: [
    keyVaultModule
  ]
}

module nsgModule './modules/11.nsg.bicep' = {
  name: 'nsg'
  params: {
    location: location
    nsgName: nsgName
    ptags: ptags
  }
}

module publicIPModule './modules/12.publicip.bicep' = {
  name: 'publicIP'
  params: {
    pipdomainNameLabel: pipdomainNameLabel
    plocation: location
    ptags: ptags
    publicIPName: publicIPName
  }
}

module vnetModule './modules/13.vnet.bicep' = {
  name: 'vnet'
  params: {
    location: location
    networkSecurityGroupId: nsgModule.outputs.nsgId
    virtualNetworkName: pvirtualNetworkName
  }
}
