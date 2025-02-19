param plocation string
param pserviceBusNamespaceName string
param pserviceBusNamespaceskuName string
param pserviceBusNamespaceskuTier string
param pserviceBusNamespaceskuCapacity int
param ptags object

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: pserviceBusNamespaceName
  location: plocation
  tags: ptags
  sku: {
    name: pserviceBusNamespaceskuName
    capacity: pserviceBusNamespaceskuCapacity
    tier: pserviceBusNamespaceskuTier
  }
}
