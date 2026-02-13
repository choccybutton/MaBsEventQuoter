# GitHub Actions Setup Guide

## Overview

This guide explains how to set up GitHub Actions for CI/CD pipeline.

## Workflows Included

### 1. CI Workflow (`ci.yml`)
**Trigger**: On PR and push to main

**Jobs**:
- Backend build, tests, formatting
- Frontend build, lint, typecheck, tests
- Code quality checks
- Security vulnerability scanning

**Services**:
- PostgreSQL (for integration tests)

**Artifacts**:
- Test results
- Coverage reports (uploaded to Codecov)

### 2. Build and Push Workflow (`build-and-push.yml`)
**Trigger**: On push to main or version tags

**Jobs**:
- Build API Docker image
- Build Frontend Docker image

**Status**: Ready for registry configuration

### 3. Quality Report Workflow (`quality-report.yml`)
**Trigger**: On PR

**Jobs**:
- Code formatting checks
- Linting and type checks
- Dependency security scan
- Coverage reporting

## Configuration

### Codecov Integration (Optional)

To track test coverage:

1. Go to [codecov.io](https://codecov.io)
2. Sign in with GitHub
3. Authorize and enable repository
4. Workflows automatically push coverage

### Docker Registry Setup (Phase 2)

To push images to registry:

1. Add secrets in GitHub repository settings:
   - `REGISTRY_USERNAME`
   - `REGISTRY_PASSWORD`
   - `REGISTRY_URL`

2. Update `build-and-push.yml`:
   ```yaml
   - name: Login to registry
     uses: docker/login-action@v2
     with:
       registry: ${{ secrets.REGISTRY_URL }}
       username: ${{ secrets.REGISTRY_USERNAME }}
       password: ${{ secrets.REGISTRY_PASSWORD }}

   - name: Build and push
     uses: docker/build-push-action@v4
     with:
       push: true  # Change from false to true
   ```

### Branch Protection Rules

Configure in repository settings:

1. Go to Settings → Branches
2. Add rule for `main` branch
3. Require:
   - ✅ Require status checks to pass before merging
   - ✅ All 4 workflows must pass
   - ✅ Require branches to be up to date
   - ✅ Require code reviews

### Environment Variables

Add in repository Settings → Secrets and variables → Variables:

```
GH_TOKEN              # GitHub token (auto-provided)
DOTNET_SKIP_FIRST_TIME_EXPERIENCE = true
DOTNET_CLI_TELEMETRY_OPTOUT = true
```

### Notifications

1. Go to Settings → Notifications
2. Configure email alerts for workflow failures

## Workflow Status

| Workflow | Status | Notes |
|----------|--------|-------|
| ci.yml | ✅ Ready | All checks enabled |
| build-and-push.yml | ⏳ Partial | Needs registry setup |
| quality-report.yml | ✅ Ready | Code quality gates |

## Running Workflows Manually

### Via CLI

```bash
# List workflows
gh workflow list

# Run workflow
gh workflow run ci.yml --ref main

# View runs
gh run list --workflow ci.yml
```

### Via Web UI

1. Go to repository → Actions
2. Select workflow
3. Click "Run workflow"
4. Choose branch
5. Click "Run"

## Monitoring Workflows

### Status Badge

Add to README:

```markdown
[![CI](https://github.com/yourusername/catering-quotes/actions/workflows/ci.yml/badge.svg)](https://github.com/yourusername/catering-quotes/actions/workflows/ci.yml)
```

### Checking Status

1. Go to repository → Actions
2. View latest run
3. Click on job to see details
4. Check logs for any failures

## Debugging Failed Workflows

1. Click on failed workflow run
2. Click on failed job
3. Expand sections to view logs
4. Look for error messages
5. Fix issues and push again

### Common Issues

**Test Failures**:
- Check PostgreSQL service health
- Verify connection string
- Review test logs

**Build Failures**:
- Check dotnet version
- Verify dependencies installed
- Check for breaking changes

**Lint Failures**:
- Run locally: `npm run lint` / `dotnet format`
- Fix issues and push

**Security Scan Issues**:
- Review Trivy scan results
- Update vulnerable dependencies
- Suppress false positives if needed

## Secrets Management

### Adding Secrets

1. Go to Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Enter name (uppercase, underscores)
4. Enter value
5. Click "Add secret"

### Using Secrets

```yaml
- name: Use secret
  env:
    MY_SECRET: ${{ secrets.MY_SECRET }}
```

## Costs

GitHub Actions is **free** for:
- Public repositories
- 2000 minutes/month for private repositories

## Performance Tips

1. **Cache dependencies**:
   - NPM: Already cached in workflows
   - NuGet: Already cached in workflows

2. **Parallel jobs**: Already configured
   - Backend and frontend test in parallel
   - Security scan parallel

3. **Fast feedback**: Lint before build

## Next Steps

1. ✅ Copy workflows to `.github/workflows/`
2. ⏳ Configure Codecov (optional)
3. ⏳ Set up branch protection rules
4. ⏳ Configure Docker registry (Phase 2)
5. ⏳ Set up deployment workflow (Phase 2)

## Reference

- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Workflow Syntax](https://docs.github.com/actions/using-workflows/workflow-syntax-for-github-actions)
- [Events that trigger workflows](https://docs.github.com/actions/using-workflows/events-that-trigger-workflows)
