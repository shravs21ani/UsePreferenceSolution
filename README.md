# User Preference Solution

A modern, cloud-native user preference management system built with Blazor WebAssembly, Azure Functions, and Azure services.

## 🏗️ Architecture

This solution implements a microservices architecture with the following components:

- **Frontend**: Blazor WebAssembly application for user interface
- **Backend API**: ASP.NET Core Web API for business logic
- **Azure Functions**: Durable Functions for event processing and orchestration
- **Azure Cosmos DB**: NoSQL database for storing user preferences
- **Azure Service Bus**: Message broker for event-driven communication
- **Azure App Configuration**: Centralized configuration management
- **Azure Key Vault**: Secure secrets and configuration storage
- **Application Insights**: Monitoring and telemetry

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Azure CLI
- Azure Subscription
- Visual Studio 2022 or VS Code

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UserPreferenceSolution
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure local development**
   - Update connection strings in `src/UserPreference.Api/appsettings.Development.json`
   - Update local settings in `src/UserPreference.Functions/local.settings.json`

4. **Run the applications**
   ```bash
   # Terminal 1: Run the API
   cd src/UserPreference.Api
   dotnet run
   
   # Terminal 2: Run the Functions
   cd src/UserPreference.Functions
   dotnet run
   
   # Terminal 3: Run the Blazor app
   cd src/UserPreference.BlazorWasm
   dotnet run
   ```

### Azure Infrastructure Deployment

1. **Login to Azure**
   ```bash
   az login
   az account set --subscription <subscription-id>
   ```

2. **Create resource group**
   ```bash
   az group create --name UserPreferenceDev --location EastUS
   ```

3. **Deploy infrastructure**
   ```bash
   az deployment group create \
     --resource-group UserPreferenceDev \
     --template-file infra/main.bicep \
     --parameters environment=dev appName=userpreference
   ```

4. **Configure secrets in Key Vault**
   ```bash
   # Get the Key Vault name from deployment output
   az keyvault secret set --vault-name <key-vault-name> --name "CosmosDbConnectionString" --value "<cosmos-db-connection-string>"
   az keyvault secret set --vault-name <key-vault-name> --name "ServiceBusConnectionString" --value "<service-bus-connection-string>"
   az keyvault secret set --vault-name <key-vault-name> --name "AppConfigConnectionString" --value "<app-config-connection-string>"
   az keyvault secret set --vault-name <key-vault-name> --name "AppInsightsConnectionString" --value "<app-insights-connection-string>"
   ```

## 📁 Project Structure

```
UserPreferenceSolution/
├── src/
│   ├── UserPreference.BlazorWasm/          # Blazor WebAssembly frontend
│   │   ├── Components/                     # Reusable Blazor components
│   │   ├── Models/                         # Data models
│   │   ├── Pages/                          # Blazor pages
│   │   ├── Services/                       # Frontend services
│   │   └── wwwroot/                        # Static assets
│   ├── UserPreference.Api/                 # ASP.NET Core Web API
│   │   ├── Controllers/                    # API controllers
│   │   ├── Data/                           # Data access services
│   │   ├── Models/                         # API models
│   │   └── Services/                       # Business logic services
│   └── UserPreference.Functions/           # Azure Functions
│       ├── Functions/                      # Function implementations
│       ├── Models/                         # Function models
│       └── Services/                       # Function services
├── infra/                                  # Infrastructure as Code
│   └── main.bicep                         # Bicep template
├── README.md                               # This file
└── UserPreferenceSolution.sln              # Solution file
```

## 🔧 Configuration

### Environment Variables

The application uses the following configuration keys:

- `CosmosDb:ConnectionString` - Azure Cosmos DB connection string
- `ServiceBus:ConnectionString` - Azure Service Bus connection string
- `AzureAppConfiguration:ConnectionString` - Azure App Configuration connection string
- `ApplicationInsights:ConnectionString` - Application Insights connection string

### Local Development

For local development, you can use:

- **Cosmos DB Emulator**: For local database testing
- **Azure Storage Emulator**: For local Functions development
- **User Secrets**: For storing sensitive configuration locally

## 🚀 Deployment

### Azure App Service Deployment

1. **Build the applications**
   ```bash
   dotnet publish -c Release
   ```

2. **Deploy to Azure App Service**
   ```bash
   # Deploy API
   az webapp deployment source config-zip \
     --resource-group UserPreferenceDev \
     --name <api-app-name> \
     --src <api-publish-path>.zip
   
   # Deploy Functions
   az functionapp deployment source config-zip \
     --resource-group UserPreferenceDev \
     --name <functions-app-name> \
     --src <functions-publish-path>.zip
   ```

### Blazor WebAssembly Deployment

The Blazor WebAssembly app can be deployed to:
- Azure Static Web Apps
- Azure Blob Storage with CDN
- Azure App Service

## 📊 Monitoring and Logging

- **Application Insights**: Application performance monitoring
- **Azure Monitor**: Infrastructure monitoring
- **Structured Logging**: Serilog integration for detailed logging

## 🔒 Security

- **Azure AD B2C**: User authentication and authorization
- **Azure Key Vault**: Secure configuration storage
- **HTTPS Only**: All endpoints use HTTPS
- **CORS**: Configured for Blazor app origins

## 🧪 Testing

### Unit Tests
```bash
dotnet test
```

### Integration Tests
```bash
dotnet test --filter Category=Integration
```

### API Testing
Use the Swagger UI at `/swagger` when running the API locally.

## 📈 Performance

- **CDN**: Static assets served from CDN
- **Caching**: Response caching for API endpoints
- **Async Operations**: Non-blocking I/O operations
- **Connection Pooling**: Optimized database connections

## 🔄 CI/CD

### GitHub Actions

The project includes GitHub Actions workflows for:
- Build and test on pull requests
- Automated deployment to staging
- Production deployment with approval

### Azure DevOps

Azure DevOps pipelines can be configured for:
- Build and test automation
- Infrastructure deployment
- Application deployment

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

## 🔮 Roadmap

- [ ] Real-time notifications with SignalR
- [ ] Multi-tenant support
- [ ] Advanced analytics dashboard
- [ ] Mobile app support
- [ ] API versioning
- [ ] GraphQL support
- [ ] Advanced caching strategies
- [ ] Disaster recovery setup
