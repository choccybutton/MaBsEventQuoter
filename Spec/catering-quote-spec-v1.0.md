# Catering Quote Application Specification
## Version 1.0 (MVP) — Code Quality Controls + Environment Seeding Strategy

> This version extends v0.9 to cover:
> 1) **Code quality control tools and gates**
> 2) A clear **data seeding strategy across Dev / Test / Production environments**

---

## 1. Code Quality Control Tools (Open Source)

### 1.1 Mandatory Standards
All code must adhere to:
- Clean Architecture boundaries (see v0.8)
- Consistent formatting and naming conventions
- Automated quality checks in CI (GitHub Actions)

All quality tooling must be **open source** to keep costs down.

---

## 2. Backend (.NET) Code Quality Tooling

### 2.1 Formatting & Style
**EditorConfig (mandatory)**
- Maintain a repo-level `.editorconfig`
- Enforce whitespace, line endings, C# style rules

**dotnet format (mandatory)**
- Run in CI:
  - `dotnet format --verify-no-changes`
- Prevent merges that introduce formatting drift

### 2.2 Static Analysis & Linting
**Roslyn analyzers (mandatory)**
Use open-source analyzers packages such as:
- `Microsoft.CodeAnalysis.NetAnalyzers` (built-in via SDK / analyzers)
- `StyleCop.Analyzers` (optional but recommended for consistency)

Configuration:
- Treat analyzer warnings as build-breaking for key rule sets (recommended):
  - `TreatWarningsAsErrors=true` (with explicit suppressions where justified)

### 2.3 Architecture/Dependency Rules
**Architecture tests (recommended)**
Add tests that enforce allowed dependencies between projects:
- Domain has no dependencies
- Application only depends on Domain
- Infrastructure depends on Application+Domain
- Api depends on Application (+ Infrastructure for wiring)

Implementation approach:
- Use xUnit tests that reflect over assemblies and validate references
- OSS libraries may be used if desired, but a minimal reflection-based approach is acceptable

### 2.4 Test Coverage Gates
**Coverage collection (mandatory)**
- Use `coverlet.collector` for .NET test coverage (OSS)
- Generate coverage reports in CI

Gate recommendation:
- Fail build if overall coverage < a defined threshold (e.g., 80%)
- Also enforce minimum per-project thresholds (optional but recommended)

Notes:
- Coverage does not replace good test quality; still require key scenario tests.

### 2.5 Security Scanning (OSS)
**Dependency vulnerability scanning (recommended)**
- Use `dotnet list package --vulnerable` in CI (OSS)
- Fail build if high/critical vulnerabilities detected (policy decision)

---

## 3. Frontend (React) Code Quality Tooling (Open Source)

### 3.1 Formatting
- **Prettier** (mandatory)
- CI runs:
  - `npm run format:check` (or equivalent)

### 3.2 Linting
- **ESLint** (mandatory)
- CI runs:
  - `npm run lint`

### 3.3 Type Safety
- TypeScript `tsc --noEmit` in CI (mandatory)

### 3.4 Unit Tests (recommended)
- Jest/Vitest (OSS) for React unit tests
- (Optional) Cypress/Playwright for end-to-end tests (OSS) — phase 2 unless required

---

## 4. GitHub Actions Quality Gates

### 4.1 Required Checks Before Merge
Minimum required checks on PR:
- Backend build
- Backend unit tests (xUnit)
- Backend integration tests (xUnit) — optionally run on PR, must run on main
- `dotnet format --verify-no-changes`
- Analyzer checks (warnings as errors where configured)
- Coverage report generation (and threshold gate)
- Frontend build
- Frontend lint
- Frontend typecheck
- Frontend format check

### 4.2 Suggested Workflow Stages
1. build-backend
2. test-unit
3. test-integration (with Postgres container)
4. quality (format/analyzers/coverage)
5. build-frontend (lint/typecheck/format)
6. docker-build-and-push
7. deploy

---

## 5. .NET Aspire Application Host (Mandatory)

### 5.1 Aspire Overview

**.NET Aspire** is the application orchestration platform for building and running cloud-native applications with .NET.

**Why Aspire:**
- Unified development and production architecture
- Built-in service orchestration (API, frontend, database)
- Integrated observability (logging, tracing, metrics)
- Resource management and health checks built-in
- Simplified local development with container support

### 5.2 Aspire Architecture

**Aspire components:**
- **AppHost project** (orchestrator): Defines all services and their relationships
- **Service Reference**: Frontend and API services registered as components
- **Resource management**: Automatic port allocation, dependency injection, health checks

**Repository structure with Aspire:**
```
/catering-quotes
  /AppHost                    # Aspire orchestration project
    Program.cs               # Define all services
    aspire-manifest.json     # Generated deployment manifest
    CateringQuotes.AppHost.csproj

  /backend
    /Api                     # ASP.NET API project
    /Application
    /Domain
    /Infrastructure
    CateringQuotes.sln

  /frontend
    /src                     # React frontend
    package.json
```

### 5.3 AppHost Configuration (Program.cs)

**Example Aspire configuration:**
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL database
var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("catering_quotes");

// Backend API
var api = builder
    .AddProject<Projects.CateringQuotes_Api>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpEndpoint(port: 5000, scheme: "http")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

// Frontend (optional in Aspire, or use Node.js component)
builder
    .AddNpmApp("frontend", "../frontend")
    .WithHttpEndpoint(port: 3000, scheme: "http")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", "http://localhost:5000")
    .WaitFor(api);

builder.Build().Run();
```

### 5.4 Development Experience with Aspire

**Local development (one command):**
```bash
dotnet run --project AppHost
```

**What happens automatically:**
- PostgreSQL container starts
- Database created and seeded
- API service starts on port 5000
- Frontend dev server starts on port 3000
- Aspire Dashboard available at http://localhost:8080
- All services health-checked and monitored

**Aspire Dashboard:**
- Real-time logs from all services
- Traces and distributed tracing
- Metrics and resource usage
- Service dependencies visualization
- Health check status

### 5.5 Environment Configuration in Aspire

**Environment-specific settings:**
- Development: Uses Aspire for full orchestration
- Test: Can use Aspire or Docker Compose for CI
- Production: Aspire manifest generates deployment configuration

**Configuration injection:**
```csharp
// In API project
var postgres = builder.Configuration.GetConnectionString("postgres");
```

Aspire automatically injects connection strings and environment variables.

### 5.6 Service Registration

**Add services to Aspire:**
```csharp
// API
var api = builder.AddProject<Projects.CateringQuotes_Api>("api");

// Services with health checks
var mailService = builder
    .AddProject<Projects.CateringQuotes_MailService>("mail-service")
    .WaitFor(postgres)
    .WithHttpHealthCheck("/health");

// Reference between services
mailService.WithReference(api);
```

### 5.7 Local Port Management

Aspire automatically manages ports:
- Postgres: 5432 (or auto-assigned)
- API: 5000 (or auto-assigned)
- Frontend: 3000 (or auto-assigned)
- Dashboard: 8080

**Override ports if needed:**
```csharp
var api = builder
    .AddProject<Projects.CateringQuotes_Api>("api")
    .WithHttpEndpoint(port: 5000);
```

### 5.8 Aspire for CI/CD

**GitHub Actions with Aspire:**
```yaml
- name: Restore dependencies
  run: dotnet restore

- name: Build
  run: dotnet build

- name: Test with Aspire
  run: |
    dotnet run --project AppHost --no-build &
    sleep 5
    dotnet test --no-build
```

**Or use Docker Compose for CI:**
- Keep docker-compose.yml as alternative for CI pipelines
- Aspire is preferred for local dev and production deployments

### 5.9 Production Deployment with Aspire

**Aspire manifest generation:**
```bash
dotnet publish --project AppHost -o ./publish
```

Generates `aspire-manifest.json` for deployment:
- Kubernetes manifests
- Container configuration
- Service dependencies
- Resource requirements

**Deployment targets:**
- Azure Container Apps (native Aspire support)
- Kubernetes (via manifest)
- Docker (manual deployment)

---

## 6. Data Seeding Strategy by Environment

We require **Dev**, **Test**, and **Production** environments.

### 5.1 Environment Definitions
- **Dev**: developer local machines + shared dev deployment (optional)
- **Test**: CI integration tests (and/or staging)
- **Production**: live environment

The API must use a clear environment indicator:
- `ASPNETCORE_ENVIRONMENT` (e.g. Development / Test / Production)

### 6.2 Seeding Categories
Define seed types:

**A) Reference/Lookup Seeds (always safe)**
- Allergens (lookup table)
- Dietary tags (lookup table)

**B) System Defaults (safe, single-row)**
- app_settings default row

**C) Example/Demo Data (dev only)**
- Example customers
- Example food items
- Example quotes (optional)

### 6.3 Seeding Rules
**Production**
- Seed only A + B.
- Never seed demo data in production.

**Dev**
- Seed A + B.
- Seed optional demo data (C) to accelerate development/testing.

**Test (CI)**
- Seed A + B only, unless a specific test requires additional data.
- Tests may insert their own data as part of arrangements.
- Test DB must be isolated and recreated per run (recommended).

### 6.4 Idempotency Requirements
All seed operations must be idempotent:
- “Insert if missing” based on unique keys (e.g., allergen.code, dietary_tag.code)
- Settings table: create single row if none exists; do not create duplicates

### 6.5 Seeding Execution Timing
**Dev/Production runtime (recommended)**
- On API startup:
  - Apply migrations (policy choice: automatic in Dev; optional in Prod)
  - Seed lookup + settings if missing

**CI Test runtime (recommended)**
- Integration test fixture:
  - Starts Postgres container
  - Applies migrations
  - Runs seeding for lookups + settings
  - Runs tests

### 6.6 Migration Policy by Environment
- **Dev**: allow automatic migrations on startup
- **Test**: apply migrations in test fixture setup
- **Production**: apply migrations as an explicit pipeline step (recommended) rather than automatic on app start

### 6.7 Developer Database Reset Utility

**Purpose:** Allow developers to quickly wipe and reseed the database during development.

**Command (MVP):**
```bash
dotnet run --project Api -- reset-db
```

**Behavior:**
1. **Development environment only** - refuse to run on Test/Production
2. Drop all tables (dangerous - hence environment check)
3. Re-apply all migrations from scratch
4. Re-seed lookup data (allergens, dietary tags, settings)
5. Optionally re-seed demo data (customers, food items, sample quotes)
6. Print confirmation message with record counts

**Implementation approach:**
```csharp
// In Program.cs or a separate command handler
if (app.Environment.IsDevelopment())
{
    app.MapPost("/reset-db", async (IServiceProvider services) =>
    {
        var dbContext = services.GetRequiredService<CateringQuotesDbContext>();

        // Drop and recreate
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        // Seed reference data
        await new DatabaseSeeder(dbContext).SeedAsync();

        return Results.Ok(new { message = "Database reset complete" });
    }).WithName("ResetDatabase").Produces(200).Produces(403);
}
```

**CLI Alternative (recommended for ease of use):**
```csharp
// Create a console command or use dotnet ef
var resetCommand = new Command("reset-db", "Wipe and reseed development database")
{
    new Option<bool>("--seed-demo", "Include demo data (customers, items, quotes)")
};

resetCommand.SetHandler(async (seedDemo) =>
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("Cannot reset database outside Development!");

    var seeder = app.Services.GetRequiredService<DatabaseSeeder>();
    await seeder.ResetAndSeedAsync(seedDemo);
}, new Binder());

app.RootCommand.Add(resetCommand);
```

**Execution options:**
```bash
# Basic reset (reference data only)
dotnet run --project Api -- reset-db

# Reset with demo data
dotnet run --project Api -- reset-db --seed-demo

# Output example
# ✅ Database reset complete
# - Allergens seeded: 14
# - Dietary tags seeded: 7
# - Settings created: 1
# - Demo customers: 5 (if --seed-demo)
# - Demo food items: 20 (if --seed-demo)
```

**Safety Guards:**
- ✅ Only works in Development environment
- ✅ Requires explicit command (not automatic)
- ✅ Confirms action before executing
- ✅ Cannot be triggered by API in Test/Production

**Developer Workflow:**
```bash
# Start development
dotnet run --project AppHost

# Later, if you mess up the database
dotnet run --project Api -- reset-db --seed-demo

# Continue developing
```

---

## 7. Seed Data Content (Baseline)

### 7.1 Settings
If `app_settings` empty:
- default_vat_rate = 0.20
- default_markup_pct = 0.70
- margin_green_threshold_pct = 0.70
- margin_amber_threshold_pct = 0.60

### 7.2 Allergens (baseline)
Codes:
- CELERY, CEREALS_GLUTEN, CRUSTACEANS, EGGS, FISH, LUPIN, MILK, MOLLUSCS, MUSTARD, NUTS, PEANUTS, SESAME, SOYA, SULPHITES

### 7.3 Dietary tags (baseline)
Codes:
- VEGAN, VEGETARIAN, GLUTEN_FREE, DAIRY_FREE, NUT_FREE, HALAL, KOSHER

---

## 8. API Documentation

### 7.1 OpenAPI/Swagger Requirement
All API endpoints must be documented using **OpenAPI 3.0** specification.

**Tooling (open source):**
- **Swashbuckle.AspNetCore** (mandatory for .NET API)
  - Generates OpenAPI specification automatically from code
  - Provides Swagger UI for interactive documentation
  - Generates ReDoc alternative documentation view

**Configuration:**
- Configure Swashbuckle in API startup:
  - Enable XML documentation comments
  - Include API version information
  - Add security definitions (JWT bearer token)
  - Document request/response schemas

### 7.2 Documentation Requirements

All API endpoints must include:
- Clear description of endpoint purpose
- Request body schema with examples (if applicable)
- Response schemas for success (200/201) and error cases (400/401/404/500)
- Authentication requirements (required/optional)
- Example request/response bodies in code comments

**Implementation:**
- Use `[ApiController]` attributes
- Use XML documentation comments (`///`) on controllers and methods
- Use `[Produces]` and `[Consumes]` attributes for clarity
- Document all request/response DTOs with `<summary>` and `<remarks>` tags

### 7.3 API Versioning Strategy

APIs must support versioning to enable breaking changes without disrupting clients.

**Approach:**
- Use **URL-based versioning** for clarity: `/api/v1/quotes`, `/api/v2/quotes`
- Default to v1 for initial MVP
- Plan for v2 when breaking changes are needed

**Implementation:**
- Use Asp.Versioning.Http NuGet package (open source, commonly used)
- Configure version routing in Startup
- Version Swagger documentation automatically per API version

### 7.4 Swagger UI Availability

- Swagger UI must be available at `/swagger` (Development environment)
- ReDoc alternative at `/redoc` (optional)
- Production: Swagger UI may be disabled or require authentication (policy decision)

### 7.5 Documentation Publishing (Phase 2)

For future:
- Consider generating static HTML documentation (e.g., using Swagger UI bundle)
- Host documentation on GitHub Pages or within app
- Maintain API changelog documenting breaking changes per version

---

## 9. Frontend Build & Artifacts

### 8.1 Build Output & Structure

**Build tool:** React application built with Vite or Create React App (CRA)
- Recommendation: **Vite** for faster build times and better optimization out-of-the-box

**Output directory:** `/frontend/dist`
- All production-ready assets generated here
- Structure:
  ```
  /dist
    /assets
      /js          (bundled JavaScript)
      /css         (bundled CSS)
      /images      (optimized images)
    index.html     (entry point)
  ```

### 8.2 Build Optimization (Mandatory)

All production builds must include:

**Code optimization:**
- Minification of JavaScript and CSS
- Tree-shaking to remove unused code
- Code splitting for lazy-loaded routes (recommended for future growth)

**Asset optimization:**
- Image optimization (WebP format with JPEG fallback)
- Image compression and responsive sizing
- CSS purging (remove unused styles)

**Output:**
- Generate source maps for debugging (optional in production, recommended in staging)
- Calculate and report bundle sizes in CI

### 8.3 Environment-Specific Configuration

Frontend must support multiple environments with configuration:

**Configuration approach:**
- Use `.env` files for environment variables:
  - `.env.development` (local development)
  - `.env.test` (CI/test environment)
  - `.env.production` (production deployment)

**Required environment variables:**
- `VITE_API_URL` (or `REACT_APP_API_URL` for CRA): API endpoint URL
- `VITE_APP_ENV`: Environment name (development, test, production)
- Additional service URLs as needed (e.g., analytics, CDN)

**Build-time replacement:**
- Environment variables embedded at build time (not runtime)
- Use `import.meta.env` (Vite) or `process.env` (CRA)

### 8.4 Build Process in CI

**GitHub Actions workflow:**
1. Install dependencies: `npm ci`
2. Run linting: `npm run lint` (already specified in section 3.2)
3. Run typecheck: `npm run typecheck` (already specified in section 3.3)
4. Run format check: `npm run format:check` (already specified in section 3.1)
5. Run unit tests: `npm run test` (optional but recommended)
6. Build production bundle: `npm run build`
7. Generate bundle analysis report (optional)
8. Push artifact to Docker image or static hosting

**Build command:**
```bash
npm run build  # Generates /dist
```

### 8.5 Artifact Handling for Deployment

**Container-based deployment (recommended):**
- Copy `/dist` into Docker container (nginx or similar static server)
- Serve from `/usr/share/nginx/html` (nginx example)
- Example Dockerfile.frontend:
  ```dockerfile
  # Build stage
  FROM node:18-alpine AS builder
  WORKDIR /app
  COPY package*.json ./
  RUN npm ci
  COPY . .
  RUN npm run build

  # Serve stage
  FROM nginx:alpine
  COPY --from=builder /app/dist /usr/share/nginx/html
  COPY nginx.conf /etc/nginx/conf.d/default.conf
  EXPOSE 80
  CMD ["nginx", "-g", "daemon off;"]
  ```

**Static hosting alternative:**
- Push `/dist` to CDN or blob storage
- Configure cache headers for assets

### 8.6 Performance Requirements

**Build performance:**
- Production build must complete in under 2 minutes
- Monitor bundle size in CI (warn if > 500KB gzipped)

**Runtime performance:**
- Lighthouse score targets (aspirational for phase 2):
  - Performance: > 80
  - Accessibility: > 90
  - Best Practices: > 90

### 8.7 Development Experience

**Local development:**
- Dev server with hot module reloading (HMR)
- Fast rebuild on file changes
- Source maps for debugging

**Command reference:**
```bash
npm run dev         # Start dev server (port 3000)
npm run build       # Production build
npm run preview     # Preview production build locally
npm run lint        # Run ESLint
npm run format      # Auto-format with Prettier
npm run test        # Run Jest/Vitest
```

---

## 10. Container & Deployment Architecture

### 9.1 Container Images

**Base images (lightweight, security-focused):**
- Backend API: `mcr.microsoft.com/dotnet/aspnet:8.0-alpine` (ASP.NET runtime)
- Frontend: `nginx:alpine` (lightweight web server)

**Image builds:**
- Multi-stage builds mandatory (separate build and runtime stages)
- Minimize layer count and final image size
- No sensitive data in image (use environment variables at runtime)

### 9.2 Image Tagging Strategy

All container images must use consistent tagging:

**Tag format:**
```
registry/service:tag
```

**Tag types:**
1. **Latest**: `api:latest`, `frontend:latest`
   - Points to most recent main branch build
   - Used for development/staging deployments

2. **Semantic versioning**: `api:1.0.0`, `frontend:1.0.0`
   - Used for production releases
   - Format: `MAJOR.MINOR.PATCH`
   - Created on tagged releases in Git

3. **Git commit SHA**: `api:abc1234`, `frontend:abc1234`
   - For traceability and debugging
   - Optional but recommended in staging

4. **Branch-based** (development): `api:main`, `api:develop`
   - For CI builds from feature branches
   - Format: `api:branch-name`

**CI implementation:**
- Main branch builds → tag as `:latest` + `:main`
- Release tags (v1.0.0) → tag as `:1.0.0` + `:latest`
- Pull requests → tag as `:pr-123` (optional)

### 9.3 Container Registry

**Recommendation:** Docker Hub or Azure Container Registry (ACR)
- Use a private registry for production images
- Tag images with full registry path: `myregistry.azurecr.io/catering-quotes/api:1.0.0`

**CI/CD integration:**
- Store registry credentials in GitHub Secrets
- Authenticate and push images in CI pipeline
- Example GitHub Actions snippet:
  ```yaml
  - name: Login to registry
    uses: docker/login-action@v2
    with:
      registry: myregistry.azurecr.io
      username: ${{ secrets.REGISTRY_USERNAME }}
      password: ${{ secrets.REGISTRY_PASSWORD }}

  - name: Build and push
    uses: docker/build-push-action@v4
    with:
      context: ./backend
      push: true
      tags: myregistry.azurecr.io/catering-quotes/api:${{ github.sha }}
  ```

### 9.4 Health Checks & Readiness Probes

**API health check endpoint (mandatory):**

Create a `/health` endpoint that returns:
- HTTP 200 OK if service is healthy
- HTTP 503 Service Unavailable if unhealthy

**Response format:**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-13T10:30:00Z",
  "version": "1.0.0"
}
```

**Checks to include:**
- Database connectivity (can connect to Postgres)
- API is responding

**Implementation:**
- Use `HealthCheckMiddleware` in .NET startup
- No authentication required on `/health`
- Fast response (< 1 second)

**Docker health check:**
```dockerfile
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1
```

**Kubernetes probes (if deployed to Kubernetes):**
```yaml
livenessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 10
  periodSeconds: 30

readinessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 5
  periodSeconds: 10
```

### 9.5 Database Connection Pooling

**PostgreSQL connection pooling (mandatory):**

Connection pools prevent database connection exhaustion.

**Configuration in .NET:**
```csharp
// In appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=postgres;Port=5432;Database=catering_quotes;Username=postgres;Password=***;Maximum Pool Size=20;Minimum Pool Size=5;"
}
```

**Pool settings guidance:**
- `Maximum Pool Size`: 20 (adjust based on deployment size; default is 100)
- `Minimum Pool Size`: 5 (keep connections warm)
- `Connection Idle Lifetime`: 300 seconds (close idle connections)
- `Connection Lifetime`: 3600 seconds (recycle connections periodically)

**Monitoring:**
- Log connection pool exhaustion warnings
- Monitor pool utilization in production
- Alert if pool near capacity

**pgAdmin (optional):**
- Include in docker-compose for local development
- Helps debug connection issues
- Never deploy to production

### 9.6 Docker Compose Configuration

**Services required:**
```yaml
version: '3.8'
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: catering_quotes
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build:
      context: ./backend
      dockerfile: Dockerfile
    ports:
      - "5000:5000"
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=catering_quotes;Username=postgres;Password=postgres;
    depends_on:
      - postgres
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 5s
      retries: 3

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:3000"
    environment:
      VITE_API_URL: http://localhost:5000
    depends_on:
      - api

  pgadmin:
    image: dpage/pgadmin4:latest
    ports:
      - "5050:80"
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@example.com
      PGADMIN_DEFAULT_PASSWORD: admin
    depends_on:
      - postgres

volumes:
  postgres_data:
```

### 9.7 Container Network Security

**Network isolation:**
- Containers communicate only through defined ports
- Database only accessible to API container (not frontend)
- Frontend to API via HTTP (port 5000 in dev, reverse proxy in prod)

**Production considerations:**
- Use ingress/reverse proxy (nginx) for frontend
- API not directly exposed to internet
- Implement TLS termination at proxy layer

### 9.8 Deployment Checklist

Before pushing container to registry:
- ✅ Image builds successfully
- ✅ No secrets embedded in image
- ✅ Health check endpoint functional
- ✅ Correct environment variables documented
- ✅ Base image is stable and security-patched
- ✅ Image size is reasonable (< 500MB for API)
- ✅ All required ports exposed and documented

---

## 11. Runtime Configuration

### 10.1 Logging Infrastructure

**Logging framework (mandatory):** Serilog
- Industry-standard structured logging for .NET
- Open source with rich ecosystem of sinks
- Structured events enable better querying and analysis

**Configuration approach:**
- Configure Serilog in `appsettings.json` per environment
- Use environment variable overrides in production

**Serilog setup (example):**
```csharp
// Program.cs
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithProperty("Application", "CateringQuotes.Api");
});
```

### 10.2 Log Levels by Environment

**Development:**
- Minimum level: **Debug**
- Includes: all application logs, framework logs, SQL queries (optional)
- Output: Console + file
- Retention: 7 days

**Test (CI):**
- Minimum level: **Information**
- Includes: application logs, warnings, errors
- Output: File only (for CI artifact collection)
- Retention: 1 day (CI cleanup)

**Production:**
- Minimum level: **Information**
- Includes: business events, warnings, errors (no debug noise)
- Output: Structured sink (Application Insights, Seq, or cloud provider)
- Retention: 30 days minimum (policy decision)

**Configuration example (appsettings.Production.json):**
```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "ApplicationInsights",
        "Args": {
          "instrumentationKey": "${APPINSIGHTS_INSTRUMENTATION_KEY}"
        }
      }
    ],
    "Properties": {
      "Application": "CateringQuotes.Api"
    }
  }
}
```

### 10.3 Structured Logging Requirements

All logs must include contextual information:

**Standard fields:**
- `Timestamp`: When event occurred
- `Level`: Debug, Information, Warning, Error, Fatal
- `Message`: Human-readable description
- `SourceContext`: Class/namespace where log originated
- `Environment`: Deployment environment
- `Application`: Application name

**Business event logging:**
Log the following business events:
- Quote creation/update/deletion
- Quote email sent (success/failure)
- PDF generation success/failure
- Margin calculation events
- User authentication events
- Validation failures

**Example:**
```csharp
logger.LogInformation(
    "Quote created: {QuoteId} for customer {CustomerId} with {ItemCount} items",
    quoteId, customerId, itemCount);

logger.LogError(
    "Failed to send quote {QuoteId} to {Email}: {ErrorMessage}",
    quoteId, email, ex.Message);
```

### 10.4 Log Output Destinations

**Development:**
- Console (immediate visibility)
- File: `/logs/app-{date}.txt` (rollover daily)

**Test (CI):**
- File only: CI can collect as artifact
- Path: `./test-logs/app-{date}.txt`

**Production (recommended options):**
- **Azure Application Insights** (if using Azure)
- **Seq** (self-hosted or cloud)
- **Datadog** (commercial, but enterprise option)
- **AWS CloudWatch** (if using AWS)
- **ELK Stack** (Elasticsearch, Logstash, Kibana)

### 10.5 Rate Limiting Strategy

**Purpose:** Prevent abuse, protect API from DDoS, ensure fair resource usage

**Implementation approach:** AspNetCore.RateLimit (open source)

**Rate limit configuration:**
```csharp
// Program.cs
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();

var rateLimitPolicy = "general";

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        policyName: rateLimitPolicy,
        configure: options =>
        {
            options.PermitLimit = 100;        // 100 requests
            options.Window = TimeSpan.FromMinutes(1);  // per minute
        });
});

app.UseRateLimiter();
```

**Rate limit rules:**
- **Default (general endpoints):** 100 requests per minute per IP
- **Authentication endpoints:** 10 requests per minute per IP (prevent brute force)
- **Public endpoints:** 200 requests per minute per IP
- **Exempt endpoints:** `/health` (no limit)

**Configuration per endpoint:**
```csharp
[HttpPost("/quotes")]
[RequireRateLimiting("general")]
public async Task<IActionResult> CreateQuote([FromBody] CreateQuoteDto dto)
{
    // Implementation
}

[HttpPost("/auth/login")]
[RequireRateLimiting("auth")]
public async Task<IActionResult> Login([FromBody] LoginDto dto)
{
    // Implementation
}
```

**Environment-specific limits:**
- Development: Disable rate limiting (easier testing)
- Test/Production: Enable with limits above

**Monitoring:**
- Log rate limit violations
- Alert if threshold exceeded repeatedly from single IP
- Consider allowing higher limits for authenticated requests (future)

### 10.6 CORS Configuration

**CORS (Cross-Origin Resource Sharing):** Required for browser-based API access

**Configuration approach:**
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("X-Total-Count");  // Expose pagination headers if used
    });
});

app.UseCors("AllowFrontend");
```

**Environment-specific origins:**

**Development (appsettings.Development.json):**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://127.0.0.1:3000"
    ]
  }
}
```

**Production (appsettings.Production.json):**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://quotes.example.com"
    ]
  }
}
```

**CORS settings:**
- `AllowAnyMethod`: Allow GET, POST, PUT, DELETE, PATCH, OPTIONS
- `AllowAnyHeader`: Allow standard headers (Content-Type, Authorization, etc.)
- `AllowCredentials`: Allow cookies/auth headers
- `WithExposedHeaders`: Expose custom response headers (e.g., pagination info)
- **Never use `AllowAnyOrigin` with `AllowCredentials`** (security risk)

**Preflight handling:**
- Browser sends OPTIONS request before actual request
- API must respond with CORS headers
- No authentication required on preflight (OPTIONS)

### 10.7 Environment Variables Reference

**Required runtime environment variables:**

```bash
# Database
ASPNETCORE_ENVIRONMENT=Development|Test|Production
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=catering_quotes;...

# Logging
Serilog__MinimumLevel__Default=Debug|Information
APPINSIGHTS_INSTRUMENTATION_KEY=<key>  # if using Application Insights

# API
ASPNETCORE_URLS=http://+:5000;https://+:5001

# CORS
Cors__AllowedOrigins__0=http://localhost:3000
Cors__AllowedOrigins__1=https://quotes.example.com

# Rate limiting
RateLimit__Enabled=true|false
RateLimit__PermitLimit=100
RateLimit__WindowMinutes=1

# JWT (if using)
Jwt__Secret=<your-secret-key>
Jwt__Issuer=CateringQuotes.Api
Jwt__Audience=CateringQuotes
```

### 10.8 Configuration Validation

On API startup, validate critical configuration:
- Database connection string is valid
- JWT secret is set (if required)
- Log directory is writable
- Rate limiting properly configured
- CORS allowed origins configured

Fail fast if configuration is missing or invalid.

---

## 12. Testing Strategy

### 11.1 Testing Pyramid Overview

Tests are organized in three layers:

1. **Unit Tests** (bottom, many): Fast, isolated, no dependencies
2. **Integration Tests** (middle, some): Test components together, use real database
3. **End-to-End Tests** (top, few): Test full workflows through UI (phase 2)

**Target coverage:**
- Unit tests: 70-80% of application logic
- Integration tests: Core workflows (quote creation, email sending, PDF generation)
- E2E tests: Critical user journeys (phase 2)

### 11.2 Backend Unit Testing

**Framework:** xUnit (open source, recommended)

**Structure:**
```
/backend/Tests
  /CateringQuotes.Api.Tests
    /Controllers
    /Services
    /Domain
```

**Unit test requirements:**
- Test single class/method in isolation
- Mock external dependencies (database, email service, PDF generation)
- Use xUnit facts/theories
- Naming convention: `MethodName_Scenario_ExpectedResult`

**Example:**
```csharp
public class QuoteCalculatorTests
{
    [Fact]
    public void CalculateMargin_ValidItems_ReturnsCorrectMargin()
    {
        // Arrange
        var calculator = new QuoteCalculator();
        var items = new[] { new QuoteItem { Cost = 100, Markup = 0.5m } };

        // Act
        var result = calculator.CalculateMargin(items);

        // Assert
        Assert.Equal(0.333m, result, precision: 3);
    }

    [Theory]
    [InlineData(0.5, 0.333)]
    [InlineData(0.7, 0.412)]
    public void CalculateMargin_VariousMarkups_ReturnsExpectedMargin(
        decimal markup, decimal expectedMargin)
    {
        var calculator = new QuoteCalculator();
        var items = new[] { new QuoteItem { Cost = 100, Markup = markup } };
        var result = calculator.CalculateMargin(items);
        Assert.Equal(expectedMargin, result, precision: 3);
    }
}
```

**Mocking library:** Moq (open source)
```csharp
[Fact]
public async Task CreateQuote_EmailServiceFails_LogsError()
{
    // Arrange
    var mockEmailService = new Mock<IEmailService>();
    mockEmailService
        .Setup(x => x.SendAsync(It.IsAny<Email>()))
        .ThrowsAsync(new Exception("SMTP error"));

    var service = new QuoteService(mockEmailService.Object);

    // Act & Assert
    await Assert.ThrowsAsync<Exception>(() => service.CreateQuoteAsync(dto));
    mockEmailService.Verify(x => x.SendAsync(It.IsAny<Email>()), Times.Once);
}
```

### 11.3 Backend Integration Testing

**Framework:** xUnit + Testcontainers (open source)

**Purpose:** Test components working together with real database

**Setup (Testcontainers):**
```csharp
using Testcontainers.PostgreSql;

public class IntegrationTestBase : IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer;
    protected HttpClient Client { get; set; }
    protected IServiceProvider ServiceProvider { get; set; }

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("catering_quotes_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _dbContainer.StartAsync();

        var connectionString = _dbContainer.GetConnectionString();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddNpgsql<CateringQuotesDbContext>(connectionString);
        builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
        // ... register other services

        var app = builder.Build();
        await app.Services.GetRequiredService<CateringQuotesDbContext>()
            .Database.MigrateAsync();

        ServiceProvider = app.Services;
        Client = new HttpClient();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
```

**Integration test example:**
```csharp
public class QuoteRepositoryIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateQuote_ValidData_PersistsToDatabase()
    {
        // Arrange
        var repository = ServiceProvider.GetRequiredService<IQuoteRepository>();
        var quote = new Quote { CustomerId = 1, Total = 1000 };

        // Act
        await repository.AddAsync(quote);
        await repository.SaveChangesAsync();

        // Assert
        var retrieved = await repository.GetByIdAsync(quote.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(1000, retrieved.Total);
    }

    [Fact]
    public async Task CreateAndEmailQuote_ValidData_SendsEmail()
    {
        // Arrange
        var quoteService = ServiceProvider.GetRequiredService<IQuoteService>();
        var dto = new CreateQuoteDto { CustomerId = 1, Email = "test@example.com" };

        // Act
        var result = await quoteService.CreateAndEmailQuoteAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.EmailSent);
    }
}
```

**Integration test requirements:**
- Isolated database per test (Testcontainers handles cleanup)
- Test realistic workflows
- Include seeding of reference data (allergens, dietary tags)
- Cover happy paths and error scenarios

**Running integration tests:**
- Local: `dotnet test --filter "Category=Integration"`
- CI: Runs in pipeline with Postgres container

### 11.4 Frontend Unit Testing

**Framework:** Jest or Vitest (open source)

**Structure:**
```
/frontend/src
  /components
    __tests__
      Component.test.tsx
  /hooks
    __tests__
      useHook.test.ts
```

**Unit test requirements:**
- Test component rendering and user interactions
- Mock API calls
- Use React Testing Library (focus on user behavior, not implementation)

**Example:**
```typescript
import { render, screen, fireEvent } from '@testing-library/react';
import { QuoteForm } from './QuoteForm';

describe('QuoteForm', () => {
  it('should submit form with valid data', async () => {
    const mockOnSubmit = jest.fn();
    render(<QuoteForm onSubmit={mockOnSubmit} />);

    fireEvent.change(screen.getByLabelText(/customer name/i), {
      target: { value: 'John Doe' }
    });
    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'john@example.com' }
    });

    fireEvent.click(screen.getByRole('button', { name: /submit/i }));

    expect(mockOnSubmit).toHaveBeenCalledWith({
      name: 'John Doe',
      email: 'john@example.com'
    });
  });

  it('should show validation error for invalid email', async () => {
    render(<QuoteForm onSubmit={jest.fn()} />);

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'invalid-email' }
    });

    fireEvent.click(screen.getByRole('button', { name: /submit/i }));

    expect(screen.getByText(/invalid email/i)).toBeInTheDocument();
  });
});
```

**Frontend test command:**
```bash
npm run test              # Run all tests
npm run test:watch       # Watch mode for development
npm run test:coverage    # Generate coverage report
```

### 11.5 End-to-End Testing (Phase 2)

**Framework recommendation:** Cypress or Playwright (open source)

**Phase 1 (MVP) scope:**
- E2E testing not required for launch
- Focus on unit and integration tests

**Phase 2 scope (post-launch):**
- Critical user journeys only
- Examples:
  - Create a quote from start to finish
  - Send quote via email
  - Download PDF quote
  - Update existing quote

**E2E test structure (example, phase 2):**
```typescript
describe('Quote Creation Workflow', () => {
  it('should create and send a quote', () => {
    cy.visit('http://localhost:3000');

    cy.get('[data-testid="new-quote-btn"]').click();
    cy.get('[data-testid="customer-select"]').select('Acme Corp');
    cy.get('[data-testid="add-item-btn"]').click();
    cy.get('[data-testid="item-cost"]').type('100');
    cy.get('[data-testid="submit-btn"]').click();

    cy.get('[data-testid="quote-number"]').should('be.visible');
    cy.get('[data-testid="send-email-btn"]').click();
    cy.get('[data-testid="success-message"]').should('contain', 'Quote sent');
  });
});
```

### 11.6 CI Test Execution

**GitHub Actions test stages:**

```yaml
build:
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v3

    - name: Build Backend
      run: dotnet build

    - name: Run Unit Tests
      run: dotnet test --filter "Category!=Integration" --no-build

    - name: Run Integration Tests
      run: dotnet test --filter "Category=Integration" --no-build
      # Testcontainers will start Postgres automatically

    - name: Generate Coverage Report
      run: dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura

    - name: Upload Coverage
      uses: codecov/codecov-action@v3

frontend:
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v3

    - name: Install Dependencies
      run: npm ci

    - name: Run Tests
      run: npm test -- --coverage

    - name: Upload Coverage
      uses: codecov/codecov-action@v3
```

### 11.7 Test Coverage Goals

**Backend:**
- Overall: ≥ 80% (enforced in CI)
- Critical paths (quote calculation, email sending): ≥ 90%
- Exclude infrastructure/configuration code

**Frontend:**
- Overall: ≥ 75% (aspirational)
- Components: ≥ 80%
- Hooks: ≥ 80%
- Utilities: ≥ 85%

**Coverage reporting:**
- Use Codecov integration for visualization
- Track coverage trends over time
- Fail build if coverage drops below threshold

### 11.8 Test Data Management

**Unit tests:**
- Use inline test data or builders
- No shared fixtures (each test is independent)

**Integration tests:**
- Seeding handled by test base class
- Reference data (allergens, dietary tags) auto-seeded
- Test-specific data created per test

**Example test builder:**
```csharp
public class QuoteBuilder
{
    private Quote _quote = new() { CustomerId = 1, Total = 1000 };

    public QuoteBuilder WithTotal(decimal total)
    {
        _quote.Total = total;
        return this;
    }

    public Quote Build() => _quote;
}

// Usage
var quote = new QuoteBuilder().WithTotal(2000).Build();
```

### 11.9 Local Test Development

**Commands:**
```bash
# Backend
dotnet test                              # All tests
dotnet test --filter "Category=Unit"    # Unit tests only
dotnet watch test                        # Watch mode (re-run on file change)

# Frontend
npm test                                 # All tests
npm test -- QuoteForm                    # Tests matching pattern
npm test -- --watch                      # Watch mode
```

---

## 13. Operational Procedures

### 12.1 Database Backup & Recovery

**Backup strategy (mandatory):**
- Automated daily backups required
- Minimum retention: 14 days (7 days for dev, 30+ days for production)
- Backup location: Separate from primary database (geo-redundant preferred)

**PostgreSQL backup approaches:**

**Option 1: Cloud-managed backups (recommended for production)**
- Azure Database for PostgreSQL: Automatic daily backups (7-35 day retention)
- AWS RDS: Automated backups with configurable retention
- Google Cloud SQL: Automatic backups with on-demand capability

**Option 2: Self-managed backups**
```bash
# Daily backup script (cron job)
#!/bin/bash
BACKUP_DIR="/backups/postgres"
DATE=$(date +%Y%m%d_%H%M%S)
pg_dump -h localhost -U postgres -d catering_quotes > \
  "$BACKUP_DIR/catering_quotes_$DATE.sql"

# Compress backup
gzip "$BACKUP_DIR/catering_quotes_$DATE.sql"

# Clean old backups (older than 14 days)
find "$BACKUP_DIR" -name "catering_quotes_*.sql.gz" -mtime +14 -delete
```

### 12.2 Backup Restoration Testing

**Backup validation (mandatory):**
- Test backup restores **monthly**
- Validate backup integrity immediately after creation
- Document recovery time objective (RTO) and recovery point objective (RPO)

**Restoration test procedure:**
1. Restore backup to staging/test environment
2. Run database consistency checks
3. Verify quote data integrity
4. Confirm all tables present and accessible
5. Document restoration time

**Documentation:**
```
Backup Restore Test Report
- Date tested: 2026-02-15
- Backup age: 7 days
- Restoration time: 45 seconds
- Database size: 2.3 GB
- Result: ✅ PASSED
- Tester: DevOps Team
```

**Alert on restore failure:**
- Immediate escalation to ops team
- Review backup and storage configuration
- Do not rely on untested backups in production

### 12.3 Secrets Management & Rotation

**Secrets to protect:**
- Database connection strings
- JWT signing keys
- Email provider API keys
- Any third-party service credentials
- Storage access keys

**Never commit to version control:**
- `.env` files
- Connection strings
- API keys
- Certificates/private keys

**Secret storage (production):**
- Azure Key Vault (if on Azure)
- AWS Secrets Manager (if on AWS)
- HashiCorp Vault (self-hosted option)
- GitHub Secrets (for CI/CD only)

**Development:**
- Use `dotnet user-secrets` for local development
  ```bash
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;..."
  dotnet user-secrets set "Jwt:Secret" "your-secret-key"
  ```

### 12.4 Secrets Rotation Policy (Phase 2)

> **Phase 2:** Post-MVP enhancement. MVP will use static secrets configured at deployment.

**Rotation schedule:**
- **JWT secret:** Every 90 days (or immediately if compromised)
- **Database password:** Every 180 days (or after personnel changes)
- **API keys:** Every 90 days or per provider policy
- **Database backups encryption key:** Every 180 days

**JWT Key Rotation (Session-Safe Approach)**

JWT key rotation requires special handling to avoid breaking active user sessions.

**Strategy: Multiple Valid Signing Keys**
- Maintain a **current signing key** and up to 2 **previous keys** simultaneously
- New tokens signed with current key only
- Accept and validate tokens signed with any of the valid keys
- Gradually retire old keys after token expiration window passes

**Implementation approach:**
```csharp
// In startup configuration
var jwtKeys = new JwtKeyRing
{
    CurrentKey = GetCurrentKey(),           // Latest key, signs new tokens
    PreviousKeys = GetPreviousKeys(count: 2) // Accept tokens from these keys
};

// Validation accepts any key in the ring
var validationParameters = new TokenValidationParameters
{
    IssuerSigningKeys = jwtKeys.AllKeys, // Current + previous keys
    ValidateIssuerSigningKey = true,
    // ... other parameters
};
```

**Rotation timeline:**
1. Generate new JWT key
2. Deploy with configuration: Current = New Key, Previous = [Old Key, Older Key]
3. New sessions use the new key
4. Existing sessions continue working (validated against previous keys)
5. After 7 days (longer than typical session TTL): remove oldest previous key
6. After 14 days: remove second previous key
7. Document rotation in audit log

**Key storage for Phase 2:**
- Store all active keys in secure vault with version/timestamp
- Automate key rotation in secrets manager
- Maintain audit trail of key rotations

**Break-glass access:**
- One senior team member has access to secrets vault
- Used only in emergencies
- All access logged and audited
- Document reason and timestamp

### 12.5 Load Testing Strategy (Phase 2)

> **Phase 2:** Post-MVP performance optimization. MVP deployment will be manual verification only.

**Purpose:** Validate system can handle expected production load

**Load testing phases:**

**Phase 1 (Pre-launch):**
- Establish baseline performance
- Identify bottlenecks
- Validate infrastructure sizing
- Test during peak hours

**Phase 2 (Post-launch, quarterly):**
- Monitor real user load
- Re-baseline with actual usage patterns
- Plan capacity for growth

**Tools (open source):**
- **k6** (JavaScript-based, recommended)
- **Apache JMeter** (GUI-based)
- **Locust** (Python-based)

**k6 test example:**
```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  vus: 100,              // 100 virtual users
  duration: '5m',        // 5 minute test
  thresholds: {
    http_req_duration: ['p(95)<500'],  // 95% requests < 500ms
    http_req_failed: ['<5%'],          // < 5% failures
  },
};

export default function () {
  // Create quote
  let createRes = http.post('https://api.example.com/api/v1/quotes', {
    customerId: 1,
    items: [{ description: 'Catering', cost: 100 }],
  }, {
    headers: { 'Authorization': `Bearer ${__ENV.AUTH_TOKEN}` },
  });

  check(createRes, {
    'create quote status is 201': (r) => r.status === 201,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });

  sleep(1);

  // List quotes
  let listRes = http.get('https://api.example.com/api/v1/quotes', {
    headers: { 'Authorization': `Bearer ${__ENV.AUTH_TOKEN}` },
  });

  check(listRes, {
    'list quotes status is 200': (r) => r.status === 200,
  });

  sleep(1);
}
```

**Load test execution:**
```bash
# Local test run
k6 run load-test.js

# Cloud test run (k6 Cloud)
k6 cloud load-test.js
```

**Load test scenarios:**

| Scenario | VUs | Duration | Purpose |
|----------|-----|----------|---------|
| Baseline | 10 | 2 min | Establish baseline |
| Normal | 50 | 5 min | Expected peak load |
| Stress | 200 | 10 min | 2x expected load |
| Spike | 500 | 1 min | Unexpected spike |
| Ramp-up | 10→500 | 10 min | Gradual load increase |

**Performance targets:**
- **95th percentile response time:** < 500ms
- **99th percentile response time:** < 1000ms
- **Error rate:** < 1%
- **Throughput:** ≥ 50 requests/second

**Monitoring during load test:**
- API response times and error rates
- Database connection pool utilization
- CPU and memory usage
- Cache hit rates
- Rate limiting activation

### 12.6 Performance Incident Response (Phase 2)

> **Phase 2:** Post-MVP operational hardening. MVP will rely on basic monitoring and manual troubleshooting.

**Performance degradation response:**
1. Alert triggered if response time > threshold or error rate > 1%
2. Check logs for errors
3. Review database queries (slow query logs)
4. Check rate limiter status
5. Review recent deployments
6. Scale resources if needed

**Post-incident:**
- Root cause analysis
- Performance optimization or infrastructure scaling
- Update load testing thresholds
- Document lessons learned

### 12.7 Database Maintenance

**Regular maintenance tasks:**

**Weekly:**
- Monitor database size and growth rate
- Check connection pool utilization
- Review error logs for database errors

**Monthly:**
- Analyze table/index statistics
- Vacuum and analyze tables (PostgreSQL):
  ```sql
  VACUUM ANALYZE;
  ```
- Review slow queries
- Backup and restore test (covered in 12.2)

**Quarterly:**
- Review and optimize indexes
- Archive old quote data (if applicable)
- Update database statistics
- Performance baseline check

**Annual (Phase 2):**
- Major version upgrade planning
- Disaster recovery drill
- Capacity planning review

### 12.8 Deployment Checklist (Pre-Production)

Before deploying to production, verify:

**Infrastructure:**
- ✅ Database backups verified and tested
- ✅ Secrets properly configured in vault
- ✅ SSL certificates valid (not expiring soon)
- ✅ DNS records configured correctly
- ✅ Load balancer health checks working

**Application:**
- ✅ All tests passing (unit, integration)
- ✅ Code quality gates passed
- ✅ No high/critical security vulnerabilities
- ✅ Database migrations tested
- ✅ Logging configured for production level
- ✅ Basic logging output verified
- ⏳ Monitoring and alerting configured (Phase 2)

**Operational (MVP - basic):**
- ✅ Basic runbook for common issues
- ✅ Team trained on deployment
- ✅ Rollback procedure tested
- ⏳ On-call rotation (Phase 2)
- ⏳ Formal incident response procedures (Phase 2)

**Post-deployment:**
- ✅ Monitor for first 24 hours
- ✅ Check all health endpoints
- ✅ Verify email sending works
- ✅ Test quote PDF generation
- ✅ Monitor database connections

### 12.9 Incident Management (Phase 2)

> **Phase 2:** Post-MVP operational framework. MVP will use manual incident tracking and ad-hoc responses.

**On-call rotation:**
- Establish 24/7 on-call coverage
- 1-week rotations recommended
- Clear escalation path

**Incident severity levels:**

| Level | Definition | Response Time | Example |
|-------|-----------|----------------|---------|
| Critical | Users cannot access system | 15 min | Database down, API error rate > 50% |
| High | Major feature broken | 1 hour | Email sending broken, quote calculation failing |
| Medium | Feature partially degraded | 4 hours | UI performance slow, intermittent errors |
| Low | Minor issue | Next business day | Typo, cosmetic UI bug |

**Incident response template:**
```
Incident Report: [Incident Title]
- Start time: [time]
- Severity: [Critical/High/Medium/Low]
- Affected component: [API/Frontend/Database]
- Root cause: [description]
- Resolution: [what was done]
- Duration: [total time]
- Post-mortem date: [scheduled date]
```

### 12.10 Monitoring & Alerting (Phase 2)

> **Phase 2:** Post-MVP observability platform. MVP will use basic logging and manual log review.

**Metrics to monitor:**
- API response times (p50, p95, p99)
- Error rate and error types
- Database query performance
- Quote creation/completion rate
- Email send success/failure rate
- System resource usage (CPU, memory, disk)
- Database connection pool utilization

**Alerts (recommended triggers):**
- API error rate > 5% for 5 minutes
- API response time p95 > 1 second for 10 minutes
- Database connections > 15/20 (near pool limit)
- Disk usage > 80%
- Email sending failure rate > 10%

**Runbook example:**
```
Alert: Email Send Failure Rate High (>10%)

1. Check email service status page
2. Review recent email logs:
   - Failed recipient addresses
   - SMTP error messages
3. Check API logs for configuration issues
4. If provider issue: post message in Slack, set status page
5. If config issue: restart API pods
6. Contact email provider support if persistent
```

---

## 14. Changes Since v0.9

This version adds:
- **.NET Aspire** as the mandatory application host and orchestration platform
- Code quality control tooling (OSS) for backend and frontend
- GitHub Actions quality gates and required checks
- Explicit environment strategy: Dev, Test, Production
- Clear seeding categories and rules per environment
- Migration policy per environment
- API documentation requirements (OpenAPI/Swagger)
- API versioning strategy (URL-based)
- Frontend build & artifacts (optimization, output handling, deployment)
- Container & deployment architecture (image tagging, health checks, connection pooling)
- Runtime configuration (logging with Serilog, rate limiting, CORS)
- Testing strategy (unit, integration, E2E phases, coverage goals)
- Operational procedures (backups, secrets rotation, load testing, monitoring)

---

## 15. Specification Status

✅ **Specification v1.0 is now COMPLETE**

This document now covers:
- **.NET Aspire** as the unified application host and orchestration platform
- Development environment and local setup with Aspire
- Repository structure and architecture with AppHost project
- Configuration and secrets management
- CI/CD pipeline requirements
- Hosting and deployment architecture
- Database operations and migrations
- Code quality and testing (with Aspire integration)
- API documentation and versioning
- Frontend build pipeline
- Container and deployment best practices
- Runtime configuration (logging, rate limiting, CORS)
- Operational procedures:
  - **MVP:** Database backups, basic secrets management, deployment checklists
  - **Phase 2:** Monitoring, alerting, incident management, on-call rotation, load testing, secrets rotation, disaster recovery

This specification is suitable for **project inception and MVP launch**. Teams can now begin implementation with clear guidance on development (using .NET Aspire), testing, deployment, and operational standards. **Aspire is mandatory** for orchestrating all services (database, API, frontend) across development, testing, and production environments.

**MVP Focus:** Core application features, code quality, testing, deployment infrastructure, and essential operational procedures.

**Phase 2 (Post-Launch):** Performance optimization, advanced monitoring, incident management framework, automated secrets rotation, and disaster recovery planning.

---

End of Specification v1.0
