using './main.bicep'

param environmentName = 'dev'
param landingzoneEnvironment = {
  resourceGroup: 'rg-spreavw-landingzone-dev-northeurope'
  containerAppsEnvironment: 'spreavw-landingzone-dev-northeurope-env'
  appConfiguration: 'spreavw-landingzone-dev-northeurope-appcfg'
  applicationInsights: 'spreavw-landingzone-dev-northeurope-ai'
  serviceBus: 'spreavw-landingzone-dev-northeurope-servicebus'
}
param corsHostnames = [
  'http://localhost:4200'
  'https://spreaview.com'
  'https://dev.spreaview.com'
  'https://www.spreaview.com'
]
param acrPassword = ''
param desiredContainerCpu = '0.25'
param desiredContainerMemory = '0.5Gi'
