using './main.bicep'

param location = 'westeurope'
param environmentName = 'dev'
param systemName = 'votr-mgmt-frontend'
param hostnames = [
  'votr-mgmt.hexmaster.nl'
]
