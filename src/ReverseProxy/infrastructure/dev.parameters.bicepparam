using './main.bicep'

param environmentName = 'dev'
param landingzoneEnvironment = {
  resourceGroup: 'rg-votr-landingzone-dev-northeurope'
  containerAppsEnvironment: 'votr-landingzone-dev-northeurope-env'
  appConfiguration: 'votr-landingzone-dev-northeurope-appcfg'
  applicationInsights: 'votr-landingzone-dev-northeurope-ai'
  serviceBus: 'votr-landingzone-dev-northeurope-servicebus'
}
param corsHostnames = [
  'http://localhost:4200'
  'http://localhost:4201'
  'https://votr-mgmt.hexmaster.nl'
  'https://votr.hexmaster.nl'
]
param acrPassword = ''
param desiredContainerCpu = '0.25'
param desiredContainerMemory = '0.5Gi'
param apiHostname = 'votr-api.hexmaster.nl'
param certificateName = 'votr-api.hexmaster.nl-votr-lan-250226080626'
