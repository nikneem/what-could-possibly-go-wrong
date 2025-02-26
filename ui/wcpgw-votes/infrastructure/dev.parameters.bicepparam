using './main.bicep'

param location = 'westeurope'
param environmentName = 'dev'
param systemName = 'votr-votes-frontend'
param hostnames = [
  'votr.hexmaster.nl'
]
