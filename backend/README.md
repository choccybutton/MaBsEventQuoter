# CateringQuotes Backend

Clean Architecture implementation with four main layers.

## Project Structure

### Domain
**Location**: `Domain/`
**Dependencies**: None

Core business entities, value objects, and interfaces. This layer contains:
- Entities (Quote, Customer, etc.)
- Value Objects (Money, Margin, etc.)
- Interfaces for repositories and services (only abstractions, no implementations)
- Business rules and validations

**Key principle**: No dependencies on external frameworks. Pure C# business logic.

### Application
**Location**: `Application/`
**Dependencies**: Domain

Business logic and orchestration. This layer contains:
- Use cases / Application services
- DTOs (Data Transfer Objects)
- Interfaces for external services (repositories, email, etc.)
- Validators
- Mappers

**Key principle**: Depends on Domain, doesn't know about Infrastructure or Web.

### Infrastructure
**Location**: `Infrastructure/`
**Dependencies**: Domain, Application

External dependencies and data access. This layer contains:
- DbContext and Entity Framework configuration
- Repository implementations
- Database migrations
- Email service implementations
- External service integrations
- Serilog configuration

**Key principle**: All database and third-party integrations live here.

### Api
**Location**: `Api/`
**Dependencies**: Application, Infrastructure

ASP.NET Core API controllers and startup configuration. This layer contains:
- Controllers
- Middleware
- Dependency injection setup
- Program.cs
- API configuration (CORS, rate limiting, etc.)

**Key principle**: Thin layer that delegates to Application services.

### Tests
**Location**: `Tests/`
**Dependencies**: All projects

Unit, integration, and architecture tests using xUnit, Moq, and Testcontainers.

## Architecture Rules

These rules are enforced:

```
✅ Domain → (no dependencies)
✅ Application → Domain
✅ Infrastructure → Application + Domain
✅ Api → Application + Infrastructure
```

## Building & Running

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Run with Aspire (from root)
dotnet run --project AppHost
```

## Adding New Features

1. **Define domain model** in Domain/
2. **Create application service** in Application/
3. **Implement repository** in Infrastructure/
4. **Add API endpoint** in Api/
5. **Write tests** in Tests/

Example: Adding a new Quote feature
```
Domain/           → Quote entity
Application/      → CreateQuoteService
Infrastructure/   → QuoteRepository, DbContext
Api/              → QuotesController
Tests/            → QuoteServiceTests, QuoteRepositoryTests
```

## Dependency Injection

All services are registered in `Api/Program.cs`. Example:

```csharp
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<CreateQuoteService>();
```
