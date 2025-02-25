using './main.bicep'

param environmentName = 'prd'
param landingzoneEnvironment = {
  resourceGroup: 'rg-spreavw-landingzone-prd-northeurope'
  containerAppsEnvironment: 'spreavw-landingzone-prd-northeurope-env'
  appConfiguration: 'spreavw-landingzone-prd-northeurope-appcfg'
  applicationInsights: 'spreavw-landingzone-prd-northeurope-ai'
  serviceBus: 'spreavw-landingzone-prd-northeurope-servicebus'
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
