param plocation string
param pworkspaceName string
param ptags object

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: pworkspaceName
  location: plocation
  tags: ptags
  properties: {
    retentionInDays: 30
  }
}
