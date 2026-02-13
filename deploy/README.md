# Deployment Guide

## Overview

This directory contains deployment configurations for the Catering Quotes application.

## Local Development with Docker Compose

### Prerequisites
- Docker Desktop installed
- 8GB+ available memory

### Start All Services

```bash
cd deploy
docker-compose up -d
```

This starts:
- PostgreSQL database on :5432
- pgAdmin on :5050 (admin@example.com / admin)
- API on :5000
- Frontend on :3000

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f frontend
```

### Stop Services

```bash
docker-compose down

# Also remove volumes
docker-compose down -v
```

## Docker Images

### Building Images Manually

**API:**
```bash
docker build -t catering-quotes-api:latest -f Dockerfile.api ../backend
docker run -p 5000:5000 -e ConnectionStrings__DefaultConnection="Host=postgres;..." catering-quotes-api:latest
```

**Frontend:**
```bash
docker build -t catering-quotes-frontend:latest -f Dockerfile.frontend ../frontend
docker run -p 3000:80 catering-quotes-frontend:latest
```

### Image Specifications

| Service | Base Image | Size | Ports |
|---------|-----------|------|-------|
| API | mcr.microsoft.com/dotnet/aspnet:8.0-alpine | ~150MB | 5000, 5001 |
| Frontend | nginx:alpine | ~40MB | 80 |
| Database | postgres:16-alpine | ~150MB | 5432 |

## Production Deployment

### Prerequisites
- Docker Registry (Docker Hub, ACR, ECR, etc.)
- Kubernetes cluster or container orchestration platform
- PostgreSQL instance (managed service recommended)
- SSL/TLS certificates

### Environment Variables (Production)

**API:**
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<production-db-connection>
Jwt__Secret=<strong-secret-key>
Jwt__Issuer=CateringQuotes.Api
Jwt__Audience=CateringQuotes
```

**Frontend:**
```
VITE_API_URL=https://api.quotes.example.com
VITE_APP_ENV=production
```

### Pushing to Registry

```bash
# Build and tag
docker build -t myregistry.azurecr.io/catering-quotes-api:1.0.0 -f Dockerfile.api ../backend
docker build -t myregistry.azurecr.io/catering-quotes-frontend:1.0.0 -f Dockerfile.frontend ../frontend

# Push
docker push myregistry.azurecr.io/catering-quotes-api:1.0.0
docker push myregistry.azurecr.io/catering-quotes-frontend:1.0.0
```

## Kubernetes Deployment (Phase 2)

Example manifest structure (to be created in Phase 2):

```
k8s/
├── api-deployment.yaml
├── api-service.yaml
├── frontend-deployment.yaml
├── frontend-service.yaml
├── postgres-secret.yaml
└── configmap.yaml
```

## Health Checks

All services include health checks:

| Service | Endpoint | Check |
|---------|----------|-------|
| API | GET /health | HTTP 200 |
| Frontend | GET /health | HTTP 200 |
| Database | pg_isready | Port 5432 |

## Security Considerations

### Frontend (nginx)
- ✅ Security headers (X-Content-Type-Options, X-Frame-Options, etc.)
- ✅ Gzip compression enabled
- ✅ Static asset caching
- ✅ SPA routing configured
- ⏳ HTTPS/TLS (configure in reverse proxy)
- ⏳ Rate limiting (configure in reverse proxy)

### API
- ✅ HTTPS on 5001
- ✅ CORS configured
- ✅ Health check endpoint
- ✅ Input validation (application layer)
- ⏳ Rate limiting (Phase 2)
- ⏳ Advanced threat protection

### Database
- ✅ Password protected
- ✅ Port restricted
- ⏳ Backup strategy (Phase 2)
- ⏳ Encryption at rest (production)
- ⏳ Replication/HA setup

## Backup Strategy

### Automated Backups (Phase 2)
- Daily PostgreSQL backups
- 14-day retention (dev), 30+ days (prod)
- Geo-redundant storage
- Monthly restore testing

### Backup Commands
```bash
# Backup database
docker exec catering_quotes_db pg_dump -U postgres catering_quotes > backup.sql

# Restore database
docker exec -i catering_quotes_db psql -U postgres catering_quotes < backup.sql
```

## Monitoring & Logging

### Docker Logs
```bash
# Follow logs
docker-compose logs -f api

# View last 100 lines
docker-compose logs --tail 100 api
```

### Application Logs
Located at `/app/logs/app-*.txt` in API container.

### Monitoring Tools (Phase 2)
- Application Insights / CloudWatch
- Prometheus + Grafana
- ELK Stack
- DataDog

## Troubleshooting

### Port Conflicts
```bash
# Check what's using port 5000
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# Use different port
docker run -p 5001:5000 catering-quotes-api:latest
```

### Database Connection Issues
```bash
# Test connection
docker exec catering_quotes_db psql -U postgres -c "SELECT 1"

# Reset data
docker-compose down -v
docker-compose up -d postgres
```

### Frontend Not Loading
1. Check nginx logs: `docker-compose logs frontend`
2. Verify API URL: Check browser console for CORS errors
3. Clear cache: `docker-compose down && docker-compose up -d`

## CI/CD Integration

Automated workflows in `.github/workflows/`:

- **ci.yml**: Build, test, quality checks on PR/push
- **build-and-push.yml**: Build Docker images on main/tags
- **quality-report.yml**: Code quality gates on PR

Workflows automatically:
1. Build and test backend/frontend
2. Run code quality checks
3. Build Docker images (on push to main)
4. Push to registry (with credentials configured)
5. Deploy (configure deployment step)

## Scaling Considerations

### Horizontal Scaling
- Frontend: Stateless, scale with load balancer
- API: Stateless, scale with load balancer
- Database: Use managed service with auto-scaling

### Caching
- Frontend: Browser cache (1 year for assets, 5 min for HTML)
- API: Redis cache layer (Phase 2)
- Database: Query optimization

### Load Balancing
- Nginx reverse proxy (recommended)
- Cloud load balancer (Azure LB, AWS ALB, GCP LB)
- Geographic distribution (CDN)

## Cost Optimization

- ✅ Use Alpine images (smaller, faster)
- ✅ Multi-stage Docker builds
- ✅ Managed database services (less ops work)
- ✅ Spot/preemptible instances (non-prod)
- ✅ Reserved instances (production)

## Deployment Checklist

Before production deployment:
- ✅ Database backups verified
- ✅ Secrets in secure vault
- ✅ SSL certificates valid
- ✅ DNS records configured
- ✅ Load balancer health checks working
- ✅ All tests passing
- ✅ Code quality gates passed
- ✅ Security scan clean
- ✅ No high/critical vulnerabilities
- ✅ Monitoring configured
- ✅ Runbooks documented
- ✅ Team trained
- ✅ Rollback procedure tested

## Support

For deployment issues, check:
1. Docker logs
2. Application logs
3. Troubleshooting section above
4. GitHub Issues
