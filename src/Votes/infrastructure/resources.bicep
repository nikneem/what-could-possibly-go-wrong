param systemName string
param defaultResourceName string
param location string
param containerVersion string
param landingzoneEnvironment object

param acrLoginServer string
param acrUsername string
@secure()
param acrPassword string
param corsHostnames array

param containerPort int = 8080
param containerAppName string

param desiredContainerCpu string
param desiredContainerMemory string

param speakerTables array = [
  'Votes'
]

resource containerAppEnvironments 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: landingzoneEnvironment.containerAppsEnvironment
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
}
resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: landingzoneEnvironment.appConfiguration
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
}
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: landingzoneEnvironment.applicationInsights
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
}

module storageAccountConfiguration '../../../infrastructure/shared/configuration-value.bicep' = {
  name: '${systemName}-config-storage'
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
  params: {
    appConfigurationName: landingzoneEnvironment.appConfiguration
    settingName: 'Votes:StorageAccountName'
    settingValue: storageAccount.name
  }
}

module speakerTablesModule '../../../infrastructure/shared/configuration-value.bicep' = [
  for table in speakerTables: {
    name: '${systemName}-config-tbl-${table}'
    scope: resourceGroup(landingzoneEnvironment.resourceGroup)
    params: {
      appConfigurationName: landingzoneEnvironment.appConfiguration
      settingName: 'Votes:${table}'
      settingValue: toLower(table)
    }
  }
]

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: uniqueString('stg${defaultResourceName}')
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  resource tableService 'tableServices' = {
    name: 'default'
    resource dirtyTable 'tables' = [
      for table in speakerTables: {
        name: toLower(table)
      }
    ]
  }
}

resource apiContainerApp 'Microsoft.App/containerApps@2023-08-01-preview' = {
  name: containerAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppEnvironments.id

    configuration: {
      activeRevisionsMode: 'Single'

      ingress: {
        external: false
        targetPort: containerPort
        transport: 'auto'
        allowInsecure: false
        traffic: [
          {
            weight: 100
            latestRevision: true
          }
        ]
        corsPolicy: {
          allowedOrigins: corsHostnames
          allowCredentials: true
          allowedHeaders: ['*']
          allowedMethods: ['*']
          maxAge: 0
        }
      }
      dapr: {
        enabled: true
        appPort: containerPort
        appId: containerAppName
      }
      secrets: [
        {
          name: 'container-registry-password'
          value: acrPassword
        }
      ]
      registries: [
        {
          server: acrLoginServer
          username: acrUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]
    }

    template: {
      containers: [
        {
          image: '${acrLoginServer}/${containerAppName}:${containerVersion}'
          name: containerAppName
          resources: {
            cpu: json(desiredContainerCpu)
            memory: desiredContainerMemory
          }
          env: [
            {
              name: 'AzureAppConfiguration__EndpointUrl'
              value: appConfiguration.properties.endpoint
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: applicationInsights.properties.ConnectionString
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 6
        rules: [
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '30'
              }
            }
          }
        ]
      }
    }
  }
}

module appConfigRoleAssignment '../../../infrastructure/shared/role-assignment-app-configuration.bicep' = {
  name: '${defaultResourceName}-appcfg-module'
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
  params: {
    containerAppPrincipalId: apiContainerApp.identity.principalId
    systemName: systemName
  }
}
module tableDataRoleAssignment '../../../infrastructure/shared/role-assignment-table-data-contrib.bicep' = {
  name: '${defaultResourceName}-tablecontrib-module'
  params: {
    containerAppPrincipalId: apiContainerApp.identity.principalId
    systemName: systemName
  }
}
