# 🚀 Simform Microservice Template

A **complete, production-ready** `dotnet new` template that generates enterprise-grade ASP.NET Core microservices with clean architecture, JWT authentication, and best practices built-in.

## 📋 What This Template Provides

Instantly generates:
- **Clean Architecture** ASP.NET Core Web API
- **JWT Authentication** with refresh tokens
- **Entity Framework Core** with Identity
- **Swagger/OpenAPI** documentation
- **Structured logging** with Serilog
- **Global exception handling**
- **Health checks** and monitoring
- **Unit tests** with xUnit and FluentAssertions

## 🎯 Business Value

✅ **Instant Project Creation** - New projects in seconds, not days  
✅ **Consistent Architecture** - All projects follow the same enterprise patterns  
✅ **Zero Configuration** - Works immediately without setup scripts  
✅ **Enterprise Ready** - Includes security, logging, validation, and tests  
✅ **Team Productivity** - Developers focus on business logic, not boilerplate  

## 🔧 Template Installation & Usage

### Installation (One-time setup)
```bash
# Install the template
dotnet new install ./SimformBoilerplateTemplate

# Verify installation
dotnet new list | findstr simform
```

### Creating New Projects
```bash
# Generate a new project (any domain)
dotnet new simform-boilerplate -n SchoolManagement
dotnet new simform-boilerplate -n HRSystem
dotnet new simform-boilerplate -n InventoryAPI

# Each generates a complete solution with:
# - SchoolManagement.sln
# - SchoolManagement.Api/
# - SchoolManagement.Database/
# - SchoolManagement.DTO/
# - SchoolManagement.Repository/
# - SchoolManagement.Service/
# - SchoolManagement.UnitTest/
# - SchoolManagement.Utility/
```

## 🏗️ Generated Project Structure

```
SchoolManagement/                    # Auto-generated project name
├── SchoolManagement.sln            # Solution file
├── SchoolManagement.Api/           # Controllers, Program.cs, Swagger
├── SchoolManagement.Database/      # EF Core, Migrations, DbContext
├── SchoolManagement.DTO/           # Data Transfer Objects
├── SchoolManagement.Repository/    # Data access layer
├── SchoolManagement.Service/       # Business logic layer
├── SchoolManagement.UnitTest/      # Unit tests
├── SchoolManagement.Utility/       # Shared utilities
├── README.md                       # Project documentation
└── .gitignore                      # Standard .NET gitignore
```

## ⚡ Quick Demo

### 1. Generate a Project
```bash
dotnet new simform-boilerplate -n HRSystem
cd HRSystem
```

### 2. Run Immediately
```bash
cd HRSystem.Api
dotnet restore
dotnet build
dotnet run
```

### 3. Test the API
- **Swagger UI**: https://localhost:7001
- **Health Check**: https://localhost:7001/health
- **API Endpoints**: `/api/v1/auth/`, `/api/v1/users/`

## 🎖️ Key Features

### ✅ Authentication & Security
- JWT token authentication with refresh tokens
- ASP.NET Core Identity integration
- Role-based authorization (Admin/User)
- Secure password policies

### ✅ Clean Architecture
- **API Layer**: Controllers and middleware
- **Service Layer**: Business logic and validation
- **Repository Layer**: Data access
- **Database Layer**: EF Core models and migrations
- **DTO Layer**: Clean data contracts

### ✅ Developer Experience
- **Auto-generated Swagger docs** for immediate API testing
- **Global exception handling** with structured error responses
- **FluentValidation** for request validation
- **Structured logging** with Serilog
- **Health checks** for monitoring

### ✅ Database & Migrations
- Entity Framework Core with Code-First approach
- Automatic database creation and seeding
- Pre-configured Identity tables and relationships


## 🔄 Distribution & Maintenance

### Organization-Wide Distribution
1. **NuGet Package** (Recommended)
   - Package the template and publish to internal NuGet feed
   - `dotnet new install YourOrg.Microservice.Template --nuget-source <feed>`

2. **Git Repository**
   - Developers clone and install: `dotnet new install ./path/to/template`

### Template Updates
- Update the template source code
- Increment version in `template.json`
- Redistribute via chosen method

## 💡 Technical Implementation

The template uses .NET's built-in templating engine with:
- **`sourceName` tokens** for dynamic replacement
- **`template.json`** for configuration and symbols
- **Post-actions** for package restoration
- **File/folder renaming** based on project name

This ensures zero manual intervention and immediate usability.

---

**Result**: Your development team can now create enterprise-ready microservices in seconds instead of days, with consistent architecture and best practices built-in from day one.
