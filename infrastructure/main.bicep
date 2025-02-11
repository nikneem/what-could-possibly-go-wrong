targetScope = 'subscription'

param location string = deployment().location
@allowed([
  'dev'
  'tst'
  'acc'
  'prd'
])
param environmentName string
param version string = '1.0.0'
@allowed([
  'Standard'
  'Free'
])
param appConfigurationSku string

var systemName = 'spreavw-landingzone'

var defaultResourceName = '${systemName}-${environmentName}-${location}'
var targetResourceGroupName = 'rg-${defaultResourceName}'

resource targetResourceGroup 'Microsoft.Resources/resourceGroups@2024-07-01' = {
  name: targetResourceGroupName
  location: location
  tags: {
    environment: environmentName
    system: systemName
    version: version
  }
}

module resourceModule 'resources.bicep' = {
  name: 'resources'
  scope: targetResourceGroup
  params: {
    location: location
    defaultResourceName: defaultResourceName
    appConfigurationSku: appConfigurationSku
  }
}
