param defaultResourceName string
param location string

@allowed([
  'Standard'
  'Free'
])
param appConfigurationSku string

param webPubSubSku string

var webPubSubVotrHub = 'votr'
var cosmosDatabaseName = 'votr'
var surveysContainer = 'surveys'
var daprStateStoreName = 'votr-state-store'
var daprPubSubName = 'votr-pub-sub'

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: 'spreaview${uniqueString('${defaultResourceName}-kv')}'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    accessPolicies: []
    publicNetworkAccess: 'Enabled'
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${defaultResourceName}-law'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${defaultResourceName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'LogAnalytics'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource redisCache 'Microsoft.Cache/redis@2023-08-01' = {
  name: '${defaultResourceName}-redis'
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    enableNonSslPort: false
    publicNetworkAccess: 'Enabled'
  }
}

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-08-02-preview' = {
  name: '${defaultResourceName}-env'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
  }
  resource stateStoreComponent 'daprComponents' = {
    name: daprStateStoreName
    properties: {
      componentType: 'state.redis'
      version: 'v1'
      secrets: [
        {
          name: 'redispassword'
          value: redisCache.listKeys().primaryKey
        }
      ]
      metadata: [
        {
          name: 'redisHost'
          value: '${redisCache.properties.hostName}:${redisCache.properties.sslPort}'
        }
        {
          name: 'redisDB'
          value: '0'
        }
        {
          name: 'redisPassword'
          secretRef: 'redispassword'
        }
        {
          name: 'enableTLS'
          value: 'true'
        }
        {
          name: 'keyPrefix'
          value: 'none'
        }
      ]
    }
  }
  resource pubsubComponent 'daprComponents' = {
    name: daprPubSubName
    properties: {
      componentType: 'pubsub.azure.servicebus.topics'
      version: 'v1'
      secrets: [
        {
          name: 'servicebusnamespace'
          value: serviceBus::accessPolicies.listKeys().primaryConnectionString
        }
      ]
      metadata: [
        {
          name: 'connectionString'
          secretRef: 'servicebusnamespace'
        }
      ]
    }
  }
}

resource redisCacheSecret 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = {
  name: 'redis-password'
  parent: keyVault
  properties: {
    value: redisCache.listKeys().primaryKey
    contentType: 'text/plain'
  }
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2023-09-01-preview' = {
  name: '${defaultResourceName}-appcfg'
  location: location
  sku: {
    name: appConfigurationSku
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }
  resource applicationInsigthsConnectionString 'keyValues@2023-09-01-preview' = {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    properties: {
      value: applicationInsights.properties.ConnectionString
    }
  }
  resource redisCacheEndpointConfiguration 'keyValues@2023-03-01' = {
    name: 'Cache:Endpoint'
    properties: {
      contentType: 'text/plain'
      value: redisCache.properties.hostName
    }
  }
  resource redisCachePasswordKeyVaultReference 'keyValues' = {
    name: 'Cache:Secret'
    properties: {
      contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
      value: '{ "uri": "${redisCacheSecret.properties.secretUri}" }'
    }
  }
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-04-15' = {
  name: '${defaultResourceName}-cdb'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
  resource database 'sqlDatabases' = {
    name: cosmosDatabaseName
    location: location
    properties: {
      resource: {
        id: cosmosDatabaseName
      }
    }
    resource configurationsContainer 'containers' = {
      name: surveysContainer
      properties: {
        resource: {
          id: surveysContainer
          partitionKey: {
            paths: [
              '/id'
            ]
            kind: 'Hash'
          }
          indexingPolicy: {
            automatic: true
            indexingMode: 'Consistent'
            includedPaths: [
              {
                path: '/*'
              }
            ]
            excludedPaths: [
              {
                path: '/"_etag"/?'
              }
            ]
          }
        }
      }
    }
  }
}

resource webPubSub 'Microsoft.SignalRService/webPubSub@2024-10-01-preview' = {
  name: '${defaultResourceName}-wps'
  location: location
  sku: {
    name: webPubSubSku
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }

  resource votrHub 'hubs' = {
    name: webPubSubVotrHub
    properties: {
      anonymousConnectPolicy: 'Allow'
    }
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2023-01-01-preview' = {
  name: '${defaultResourceName}-servicebus'
  location: location
  sku: { name: 'Standard' }
  properties: {
    minimumTlsVersion: '1.2'
  }
  resource accessPolicies 'AuthorizationRules' = {
    name: 'DaprComponentPolicy'
    properties: {
      rights: [
        'Send'
        'Listen'
        'Manage'
      ]
    }
  }
}

resource serviceBusNamespace 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'ServiceBus:Namespace'
  parent: appConfiguration
  properties: {
    value: serviceBus.name
  }
}
resource serviceBusFqdn 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'ServiceBus:Fqdn'
  parent: appConfiguration
  properties: {
    value: serviceBus.properties.serviceBusEndpoint
  }
}
resource cosmosEndpoint 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'AzureServices:CosmosDbEndpoint'
  parent: appConfiguration
  properties: {
    value: cosmosDb.properties.documentEndpoint
  }
}
resource cosmosConnectionString 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'ConnectionStrings:surveys'
  parent: appConfiguration
  properties: {
    value: 'AccountEndpoint=${cosmosDb.properties.documentEndpoint}'
  }
}
resource cacheConnectionString 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'ConnectionStrings:cache'
  parent: appConfiguration
  properties: {
    value: '${redisCache.properties.hostName}:6380,password=${redisCache.listKeys().primaryKey},ssl=True,abortConnect=False'
  }
}
resource cosmosDatabase 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'AzureServices:CosmosDbDatabase'
  parent: appConfiguration
  properties: {
    value: cosmosDatabaseName
  }
}
resource configContainer 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'AzureServices:SurveysContainer'
  parent: appConfiguration
  properties: {
    value: surveysContainer
  }
}
resource configPubSubEndpoint 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'AzureServices:WebPubSub'
  parent: appConfiguration
  properties: {
    value: 'https://${webPubSub.properties.hostName}'
  }
}
resource configPubSubEndpointConnectionString 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'ConnectionStrings:webpubsub'
  parent: appConfiguration
  properties: {
    value: 'https://${webPubSub.properties.hostName}'
  }
}
resource configPubSubHub 'Microsoft.AppConfiguration/configurationStores/keyValues@2023-09-01-preview' = {
  name: 'AzureServices:WebPubSubHub'
  parent: appConfiguration
  properties: {
    value: webPubSubVotrHub
  }
}
