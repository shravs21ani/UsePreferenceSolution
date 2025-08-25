@description('Location for all resources')
param location string = resourceGroup().location

@description('Name prefix for all resources')
param appName string = 'userpreference'

@description('Environment (dev, staging, prod)')
param environment string = 'dev'

@description('SKU for App Service Plan')
@allowed(['B1', 'B2', 'B3', 'P1v2', 'P2v2', 'P3v2'])
param appServicePlanSku string = 'P1v2'

@description('Cosmos DB consistency level')
@allowed(['Eventual', 'ConsistentPrefix', 'Session', 'BoundedStaleness', 'Strong'])
param cosmosDbConsistencyLevel string = 'Session'

@description('Enable Application Insights')
param enableApplicationInsights bool = true

@description('Enable Azure Key Vault')
param enableKeyVault bool = true

@description('Enable Azure App Configuration')
param enableAppConfiguration bool = true

// Variables
var resourceName = '${appName}-${environment}'
var tags = {
  Environment: environment
  Application: appName
  ManagedBy: 'Bicep'
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: '${resourceName}-plan'
  location: location
  tags: tags
  sku: {
    name: appServicePlanSku
    tier: appServicePlanSku == 'B1' || appServicePlanSku == 'B2' || appServicePlanSku == 'B3' ? 'Basic' : 'PremiumV2'
  }
  kind: 'app'
}

// Web API App Service
resource webApi 'Microsoft.Web/sites@2022-03-01' = {
  name: '${resourceName}-api'
  location: location
  tags: tags
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'CosmosDb__ConnectionString'
          value: cosmosDb.properties.documentEndpoint
        }
        {
          name: 'ServiceBus__ConnectionString'
          value: serviceBus.properties.serviceBusEndpoint
        }
        {
          name: 'AzureAppConfiguration__ConnectionString'
          value: appConfig.properties.endpoint
        }
        {
          name: 'ApplicationInsights__ConnectionString'
          value: insights.properties.ConnectionString
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Functions App Service
resource functionsApp 'Microsoft.Web/sites@2022-03-01' = {
  name: '${resourceName}-func'
  location: location
  tags: tags
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccount.properties.primaryEndpoints.blob
        }
        {
          name: 'ServiceBusConnectionString'
          value: serviceBus.properties.serviceBusEndpoint
        }
        {
          name: 'ApplicationInsightsConnectionString'
          value: insights.properties.ConnectionString
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Cosmos DB Account
resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2023-09-15' = {
  name: '${resourceName}-cosmos'
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: cosmosDbConsistencyLevel
    }
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
}

// Cosmos DB Database
resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-09-15' = {
  name: 'UserPreferences'
  parent: cosmosDb
  properties: {
    resource: {
      id: 'UserPreferences'
    }
  }
}

// Cosmos DB Container
resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2023-09-15' = {
  name: 'Preferences'
  parent: cosmosDatabase
  properties: {
    resource: {
      id: 'Preferences'
      partitionKey: {
        paths: ['/id']
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        automatic: true
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

// Azure Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = if (enableKeyVault) {
  name: '${resourceName}-kv'
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: webApi.identity.principalId
        permissions: {
          secrets: ['get', 'list']
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: functionsApp.identity.principalId
        permissions: {
          secrets: ['get', 'list']
        }
      }
    ]
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
    enableRbacAuthorization: false
  }
}

// Azure Service Bus Namespace
resource serviceBus 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: '${resourceName}-sb'
  location: location
  tags: tags
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

// Service Bus Topic
resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  name: 'user-preferences'
  parent: serviceBus
  properties: {
    maxSizeInMegabytes: 5120
    defaultMessageTimeToLive: 'P14D'
    enableBatchedOperations: true
    enableExpress: false
    enablePartitioning: false
    supportOrdering: false
  }
}

// Service Bus Topic Authorization Rule
resource serviceBusAuthRule 'Microsoft.ServiceBus/namespaces/topics/authorizationRules@2022-10-01-preview' = {
  name: 'RootManageSharedAccessKey'
  parent: serviceBusTopic
  properties: {
    rights: ['Manage', 'Send', 'Listen']
  }
}

// Azure App Configuration
resource appConfig 'Microsoft.AppConfiguration/configurationStores@2023-03-01-preview' = if (enableAppConfiguration) {
  name: '${resourceName}-ac'
  location: location
  tags: tags
  sku: {
    name: 'standard'
  }
}

// Application Insights
resource insights 'Microsoft.Insights/components@2020-02-02' = if (enableApplicationInsights) {
  name: '${resourceName}-ai'
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: ''
  }
}

// Storage Account for Functions
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: '${resourceName}storage'
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

// Outputs
output apiUrl string = webApi.properties.defaultHostName
output functionUrl string = functionsApp.properties.defaultHostName
output cosmosDbEndpoint string = cosmosDb.properties.documentEndpoint
output keyVaultUri string = keyVault.properties.vaultUri
output serviceBusNamespace string = serviceBus.name
output appConfigEndpoint string = appConfig.properties.endpoint
output appInsightsInstrumentationKey string = insights.properties.InstrumentationKey
output storageAccountName string = storageAccount.name
