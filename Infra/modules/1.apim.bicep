@description('The name of the API Management service instance')
param papiManagementServiceName string

@description('The email address of the owner of the service')
@minLength(1)
param pAPIMpublisherEmail string

@description('The name of the owner of the service')
@minLength(1)
param pAPIMpublisherName string

@description('The pricing tier of this API Management service')
@allowed([
  'Consumption'
  'Developer'
  'Basic'
  'Basicv2'
  'Standard'
  'Standardv2'
  'Premium'
])
param pAPIMsku string 

@description('The instance size of this API Management service.')
@allowed([
  0
  1
  2
])
param pAPIMskuCount int = 1

@description('Location for all resources.')
param plocation string = resourceGroup().location

@description('Tags for resources.')
param ptags object


resource apiManagementService 'Microsoft.ApiManagement/service@2024-06-01-preview' = {
  name: papiManagementServiceName
  location: plocation
  tags: ptags
  sku: {
    name: pAPIMsku
    capacity: pAPIMskuCount
  }
  properties: {
    publisherEmail: pAPIMpublisherEmail
    publisherName: pAPIMpublisherName
  }
}
