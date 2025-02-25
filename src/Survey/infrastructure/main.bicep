targetScope = 'subscription'

var systemName = 'spreavw-rvws'

@allowed([
  'dev'
  'tst'
  'acc'
  'prd'
])
param environmentName string
param location string = deployment().location
param containerVersion string = '1.0.0'
param landingzoneEnvironment object
param acrLoginServer string = 'passed-from-workflow'
param acrUsername string = 'passed-from-workflow'
@secure()
param acrPassword string
param corsHostnames array

param desiredContainerCpu string
param desiredContainerMemory string
param containerAppName string = 'passed-from-workflow'

var defaultResourceName = toLower('${systemName}-${environmentName}-${location}')
var apiResourceGroupName = 'rg-${defaultResourceName}'

resource apiResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: apiResourceGroupName
  location: location
  tags: {
    environment: environmentName
    system: systemName
    version: containerVersion
  }
}

module resourcesModule 'resources.bicep' = {
  name: '${systemName}-resources'
  scope: apiResourceGroup
  params: {
    systemName: systemName
    defaultResourceName: defaultResourceName
    location: location
    containerVersion: containerVersion
    landingzoneEnvironment: landingzoneEnvironment
    acrLoginServer: acrLoginServer
    acrUsername: acrUsername
    acrPassword: acrPassword
    corsHostnames: corsHostnames
    desiredContainerCpu: desiredContainerCpu
    desiredContainerMemory: desiredContainerMemory
    containerAppName: containerAppName
  }
}

output resourceGroupName string = apiResourceGroup.name
