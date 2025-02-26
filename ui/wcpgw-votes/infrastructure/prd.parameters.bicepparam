using './main.bicep'

param location = 'westeurope'
param environmentName = 'prd'
param systemName = 'spreavw-frontend'
param hostnames = [
  'spreaview.com'
  'www.spreaview.com'
]
