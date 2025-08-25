# GitHub Actions Workflows

This directory contains GitHub Actions workflows for CI/CD automation of the UserPreferenceSolution.

## 🚀 Workflows Overview

### 1. **CI/CD Pipeline** (`ci-cd.yml`)
Main workflow that runs on every push to main/develop branches and pull requests.

**Jobs:**
- **Build and Test**: Builds all projects, runs tests, and publishes artifacts
- **Deploy Infrastructure**: Deploys Azure resources using Bicep templates
- **Deploy Blazor**: Deploys Blazor WASM to Azure Storage static website

**Triggers:**
- Push to main/develop branches
- Pull requests to main/develop branches
- Manual workflow dispatch

### 2. **Test Suite** (`test.yml`)
Dedicated workflow for running tests on pull requests and pushes.

**Features:**
- Matrix testing with different .NET versions
- Code coverage collection
- Integration with Codecov

### 3. **Security Scan** (`security-scan.yml`)
Security scanning workflow that runs weekly and on security-related changes.

**Features:**
- Dependency vulnerability scanning
- Security audit
- Snyk integration
- Scheduled weekly runs

### 4. **Infrastructure Deployment** (`infrastructure.yml`)
Dedicated workflow for Azure infrastructure deployment.

**Features:**
- Bicep template validation
- Infrastructure deployment
- Manual environment selection
- Deployment outputs

## 🔑 Required Secrets

Set these secrets in your GitHub repository settings:

### Azure Authentication
```bash
AZURE_CREDENTIALS          # Service principal credentials (JSON)
AZURE_RESOURCE_GROUP       # Target resource group name
AZURE_LOCATION            # Azure region (e.g., eastus)
```

### Azure Deployment
```bash
AZURE_WEBAPP_PUBLISH_PROFILE      # Web App publish profile
AZURE_FUNCTIONAPP_PUBLISH_PROFILE # Functions publish profile
AZURE_STORAGE_ACCOUNT            # Storage account name
```

### Security Scanning
```bash
SNYK_TOKEN                # Snyk API token (optional)
```

## 🛠️ Setup Instructions

### 1. Create Azure Service Principal
```bash
az ad sp create-for-rbac --name "github-actions" --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} \
  --sdk-auth
```

### 2. Add Repository Secrets
1. Go to your GitHub repository
2. Navigate to Settings → Secrets and variables → Actions
3. Add the required secrets listed above

### 3. Configure Branch Protection
1. Go to Settings → Branches
2. Add rule for `main` branch
3. Enable "Require status checks to pass before merging"
4. Select the required status checks:
   - `build-and-test`
   - `test`
   - `security-scan`

## 📋 Workflow Execution

### Automatic Triggers
- **Push to main**: Full CI/CD pipeline
- **Push to develop**: Build and test only
- **Pull Request**: Test suite and security scan
- **Infrastructure changes**: Infrastructure validation and deployment

### Manual Triggers
- **CI/CD Pipeline**: Manual deployment trigger
- **Infrastructure Deployment**: Manual infrastructure deployment with environment selection

## 🔍 Monitoring and Debugging

### Workflow Status
- Check Actions tab for workflow execution status
- View detailed logs for each step
- Monitor deployment progress

### Common Issues
1. **Azure Authentication**: Verify service principal permissions
2. **Build Failures**: Check .NET version compatibility
3. **Deployment Errors**: Verify Azure resource group and permissions
4. **Secret Issues**: Ensure all required secrets are properly configured

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure CLI GitHub Actions](https://github.com/azure/login)
- [Bicep Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
