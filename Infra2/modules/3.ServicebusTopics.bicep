param pserviceBusNamespaceName string
param penablePartitioning bool
param topicsAndSubscriptions array


resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: pserviceBusNamespaceName
}

resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' =  [ for topic in topicsAndSubscriptions: {
  parent: serviceBusNamespace
  name: topic.name
  properties: {
    enablePartitioning: penablePartitioning
    duplicateDetectionHistoryTimeWindow: 'PT1H'
    enableBatchedOperations: true
    enableExpress: false
    requiresDuplicateDetection: false
    status: 'Active'
    supportOrdering: false
  }
}]


// Authorization Rules as a separate resource
module subs './4.ServicebusTopicSubscription.bicep' = [ for topic in topicsAndSubscriptions: {
  name: '${topic.name}-subs'
  params: {
    servicebusNamespaceName: pserviceBusNamespaceName
    topicName: topic.name
    subscriptions: topic.subscriptions
  }
}]


