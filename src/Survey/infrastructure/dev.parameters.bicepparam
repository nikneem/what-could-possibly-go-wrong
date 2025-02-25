using './main.bicep'

param environmentName = 'dev'
param landingzoneEnvironment = {
  resourceGroup: 'rg-votr-landingzone-dev-northeurope'
  containerAppsEnvironment: 'votr-landingzone-dev-northeurope-env'
  appConfiguration: 'votr-landingzone-dev-northeurope-appcfg'
  applicationInsights: 'votr-landingzone-dev-northeurope-ai'
  serviceBus: 'votr-landingzone-dev-northeurope-servicebus'
  cosmosDb: 'votr-landingzone-dev-northeurope-cdb'
}
param corsHostnames = [
  'http://localhost:4200'
  'http://localhost:4201'
]
param acrPassword = ''
param desiredContainerCpu = '0.25'
param desiredContainerMemory = '0.5Gi'
