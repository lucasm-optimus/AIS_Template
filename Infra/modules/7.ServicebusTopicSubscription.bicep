param servicebusNamespaceName string
param topicName string
param subscriptions array 


resource sub 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = [for i in subscriptions: {
  name: '${servicebusNamespaceName}/${topicName}/${i}'
  properties: {
    lockDuration: 'PT1M'
    requiresSession: false
    defaultMessageTimeToLive: 'P7D'
    deadLetteringOnMessageExpiration: false
    deadLetteringOnFilterEvaluationExceptions: false
    maxDeliveryCount: 1
    status: 'Active'
  }
}]
