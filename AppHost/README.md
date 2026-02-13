# CateringQuotes.AppHost

The Aspire application orchestration project. This project defines and coordinates all services (API, database, frontend) for development and deployment.

## Running Locally

```bash
# From the root directory
dotnet run --project AppHost
```

This will:
- Start PostgreSQL container with pgAdmin
- Create the `catering_quotes` database
- Start the API on ports 5000 (HTTP) and 5001 (HTTPS)
- Launch the Aspire Dashboard at http://localhost:8080

## Services

### PostgreSQL
- **Container**: postgres:16-alpine
- **Database**: catering_quotes
- **User**: postgres
- **Port**: 5432
- **pgAdmin**: http://localhost:5050 (admin@example.com / admin)

### API
- **Project**: CateringQuotes.Api
- **HTTP Port**: 5000
- **HTTPS Port**: 5001
- **URL**: http://localhost:5000

### Frontend (Phase 2)
- **Project**: React app
- **Port**: 3000 (when enabled)
- **URL**: http://localhost:3000

## Aspire Dashboard

Monitor all services in real-time at: http://localhost:8080

- View logs from all services
- Check service health
- Monitor resource usage
- View distributed traces
