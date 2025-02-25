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

param apiHostname string
param certificateName string

resource containerAppEnvironments 'Microsoft.App/managedEnvironments@2024-10-02-preview' existing = {
  name: landingzoneEnvironment.containerAppsEnvironment
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
  resource certificate 'managedCertificates' existing = {
    name: certificateName
  }
}
resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: landingzoneEnvironment.appConfiguration
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
}
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: landingzoneEnvironment.applicationInsights
  scope: resourceGroup(landingzoneEnvironment.resourceGroup)
}

resource apiContainerApp 'Microsoft.App/containerApps@2024-10-02-preview' = {
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
        external: true
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
        // customDomains: [
        //   {
        //     name: apiHostname
        //     bindingType: 'SniEnabled'
        //     certificateId: containerAppEnvironments::certificate.id
        //   }
        // ]
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
                concurrentRequests: '50'
              }
            }
          }
        ]
      }
    }
  }
}
