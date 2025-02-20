param plocation string 
param pappInsightsName string
param ptags object

resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: pappInsightsName
  location: plocation
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
  tags: ptags
}

output oInstrumentationKey string = appInsightsComponents.properties.InstrumentationKey
output oAppInsightsId string = appInsightsComponents.id
