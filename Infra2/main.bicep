@description('Resource plocation')
param location string = resourceGroup().location

@description('Key Vault Name')
param pkeyVaultName string 

@description('Service Bus Namespace Name')
param pserviceBusNamespaceName string 

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


@description('The name of the topicsAndSubscriptions workspace')
param topicsAndSubscriptions array

@description('The name of the secrets')
param psecrets array 

module apimModule './modules/1.apim.bicep' = {
  name: 'apimName'
  params: {
    papiManagementServiceName: papiManagementServiceName
    pAPIMpublisherEmail: pAPIMpublisherEmail
    pAPIMpublisherName: pAPIMpublisherName
    ptags: ptags
    pAPIMsku: pAPIMsku
    plocation: location
  }
}


module serviceBusTopicsModule './modules/3.ServicebusTopics.bicep' = {
  name: 'serviceBusTopics'
  params: {
    penablePartitioning: penablePartitioning
    pserviceBusNamespaceName: pserviceBusNamespaceName
    topicsAndSubscriptions: topicsAndSubscriptions
  }
}

module keyVaultSecretsModule './modules/2.KeyVaultSecrets.bicep' = {
  name: 'keyVaultSecrets'
  params: {
    pkeyVaultName: pkeyVaultName
    psecrets: psecrets
  }
}
