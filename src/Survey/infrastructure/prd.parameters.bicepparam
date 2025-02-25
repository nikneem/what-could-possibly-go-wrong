using './main.bicep'

param environmentName = 'prd'
param landingzoneEnvironment = {
  resourceGroup: 'rg-votr-landingzone-prd-northeurope'
  containerAppsEnvironment: 'votr-landingzone-prd-northeurope-env'
  appConfiguration: 'votr-landingzone-prd-northeurope-appcfg'
  applicationInsights: 'votr-landingzone-prd-northeurope-ai'
  serviceBus: 'votr-landingzone-prd-northeurope-servicebus'
  cosmosDb: 'votr-landingzone-prd-northeurope-cdb'
}
param corsHostnames = [
  'http://localhost:4200'
  'http://localhost:4201'
]
param acrPassword = ''
param desiredContainerCpu = '0.25'
param desiredContainerMemory = '0.5Gi'
