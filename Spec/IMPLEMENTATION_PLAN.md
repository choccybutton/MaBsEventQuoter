# Catering Quotes MVP - Implementation Plan

**Status**: In Progress (Foundation Phase - 40% complete)
**Last Updated**: 2026-02-13
**Version**: 1.0

## Executive Summary

Comprehensive implementation plan for the Catering Quotes application MVP. This document outlines all work required to deliver a production-ready system for managing catering quotes with pricing, margins, and email delivery.

### Current Progress

**✅ COMPLETED (40%)**:
- Specification v1.0 finalized
- Aspire AppHost orchestration setup
- Backend Clean Architecture (Domain, Application, Infrastructure, Api)
- Database schema with EF Core migrations
- Frontend React skeleton with TypeScript
- CI/CD GitHub Actions workflows
- Docker containerization
- Documentation and deployment guides

**🔄 IN PROGRESS**: Foundation integration and testing

**⏳ TODO (60%)**: API endpoints, business logic, UI components, advanced features

---

## Phase 1: Foundation (MVP Baseline) - 40-50% Complete

### 1.1 Backend Core API Endpoints

**Status**: Not Started

**Controllers & DTOs**:
- [ ] **QuotesController** (`/api/v1/quotes`)
  - `GET /api/v1/quotes` - List all quotes (with filtering, pagination)
  - `GET /api/v1/quotes/{id}` - Get single quote
  - `POST /api/v1/quotes` - Create new quote
  - `PUT /api/v1/quotes/{id}` - Update quote
  - `DELETE /api/v1/quotes/{id}` - Delete quote
  - `POST /api/v1/quotes/{id}/send` - Send quote via email
  - `GET /api/v1/quotes/{id}/pdf` - Generate PDF (Phase 2)

- [ ] **CustomersController** (`/api/v1/customers`)
  - `GET /api/v1/customers` - List customers
  - `GET /api/v1/customers/{id}` - Get customer
  - `POST /api/v1/customers` - Create customer
  - `PUT /api/v1/customers/{id}` - Update customer
  - `DELETE /api/v1/customers/{id}` - Delete customer

- [ ] **FoodItemsController** (`/api/v1/food-items`)
  - `GET /api/v1/food-items` - List food items
  - `GET /api/v1/food-items/{id}` - Get item
  - `POST /api/v1/food-items` - Create item
  - `PUT /api/v1/food-items/{id}` - Update item
  - `DELETE /api/v1/food-items/{id}` - Delete item

- [ ] **ReferenceDataController** (`/api/v1`)
  - `GET /api/v1/allergens` - List allergens
  - `GET /api/v1/dietary-tags` - List dietary tags

- [ ] **SettingsController** (`/api/v1/settings`)
  - `GET /api/v1/settings` - Get application settings
  - `PUT /api/v1/settings` - Update settings

**DTOs** (Data Transfer Objects):
- [ ] CreateQuoteDto, UpdateQuoteDto, QuoteDto
- [ ] CreateCustomerDto, UpdateCustomerDto, CustomerDto
- [ ] CreateFoodItemDto, UpdateFoodItemDto, FoodItemDto
- [ ] QuoteLineItemDto
- [ ] PaginatedResponse<T>
- [ ] ApiErrorResponse

**Estimated Effort**: 3-4 days

---

### 1.2 Application Layer Services

**Status**: Not Started

**Quote Services**:
- [ ] `CreateQuoteService`
  - Generate unique quote number
  - Calculate totals (cost, price, margin)
  - Validate line items
  - Apply settings defaults (VAT, markup)

- [ ] `UpdateQuoteService`
  - Recalculate pricing on changes
  - Validate state transitions (Draft → Sent → Accepted)
  - Prevent updates to sent quotes (business rule)

- [ ] `DeleteQuoteService`
  - Only allow deletion of Draft quotes
  - Cascade delete line items

- [ ] `QuotePricingService`
  - Calculate margin percentage
  - Apply markup to cost
  - Calculate VAT
  - Determine margin status (green/amber/red)

- [ ] `QuoteEmailService` (Phase 2)
  - Format quote for email
  - Attach PDF
  - Track send status
  - Log delivery failures

**Customer Services**:
- [ ] `CreateCustomerService` - With validation
- [ ] `UpdateCustomerService` - Prevent email duplicates
- [ ] `GetCustomerQuotesService` - Retrieve all quotes for customer

**Food Item Services**:
- [ ] `CreateFoodItemService` - With validation
- [ ] `UpdateFoodItemService`
- [ ] `BulkImportFoodItemsService` (Phase 2)

**Validation**:
- [ ] Quote validation (line items, pricing)
- [ ] Customer validation (email format, required fields)
- [ ] Food item validation (cost > 0)

**Estimated Effort**: 4-5 days

---

### 1.3 Repository Pattern & Data Access

**Status**: Partially Complete (base interfaces defined)

**Repositories to Implement**:
- [ ] `IQuoteRepository` implementation
  - GetById, GetAll, Create, Update, Delete
  - GetByQuoteNumber (unique lookup)
  - GetByCustomerId (filter by customer)
  - SaveChangesAsync

- [ ] `ICustomerRepository` implementation
  - GetByEmail (unique lookup)
  - GetWithQuotes (eager load quotes)

- [ ] `IFoodItemRepository` implementation
  - GetActive only (IsActive filter)

- [ ] `IUnitOfWork` pattern
  - Coordinate multiple repositories
  - Transaction management

**Estimated Effort**: 2-3 days

---

### 1.4 Input Validation & Error Handling

**Status**: Not Started

**Validation**:
- [ ] Quote validation rules
  - Minimum 1 line item
  - All items must have quantity > 0
  - Customer required
  - Positive totals

- [ ] Customer validation
  - Email unique
  - Email valid format
  - Name required

- [ ] Food item validation
  - Name required
  - Cost price > 0

**Error Handling**:
- [ ] Custom exception types (DomainException, ValidationException)
- [ ] Global exception middleware
- [ ] Standardized error responses
- [ ] Proper HTTP status codes

**Estimated Effort**: 2-3 days

---

### 1.5 Unit & Integration Tests (Backend)

**Status**: Framework setup complete, tests needed

**Unit Tests**:
- [ ] QuotePricingService tests (15+ scenarios)
- [ ] Quote validation tests
- [ ] Customer validation tests
- [ ] Margin calculation tests

**Integration Tests**:
- [ ] End-to-end quote creation flow
- [ ] Customer CRUD operations
- [ ] Quote update with recalculation
- [ ] Concurrency scenarios

**Coverage Target**: 80%+

**Estimated Effort**: 3-4 days

---

### 1.6 Frontend Core Pages & Components

**Status**: Skeleton complete, components needed

**Page Components**:
- [ ] **Dashboard** (`/`)
  - Summary stats (recent quotes, pending emails)
  - Quick actions
  - Link to main features

- [ ] **Quotes List** (`/quotes`)
  - Table with all quotes
  - Filter by status, customer
  - Pagination
  - Actions: View, Edit, Delete, Send

- [ ] **Quote Detail** (`/quotes/:id`)
  - Full quote information
  - Line items table
  - Pricing breakdown
  - Margin indicator (green/amber/red)
  - Action buttons

- [ ] **Create/Edit Quote** (`/quotes/new`, `/quotes/:id/edit`)
  - Form with customer selection
  - Add/remove line items
  - Real-time pricing calculation
  - Save as draft
  - Validation feedback

- [ ] **Customers** (`/customers`)
  - List of customers
  - Create customer form
  - Edit customer
  - View customer quotes

- [ ] **Food Items** (`/food-items`)
  - Catalog of food items
  - Create/edit items
  - Mark inactive
  - Filter by dietary/allergen info

- [ ] **Settings** (`/settings`)
  - VAT rate
  - Markup percentage
  - Margin thresholds
  - Read-only for MVP

**Reusable Components**:
- [ ] QuoteForm (form with validation)
- [ ] LineItemsTable (display + edit)
- [ ] CustomerSelect (searchable dropdown)
- [ ] FoodItemSelect (searchable dropdown)
- [ ] PriceDisplay (formatted currency)
- [ ] MarginIndicator (color coded)
- [ ] Pagination controls
- [ ] Loading spinner
- [ ] Error alert
- [ ] Confirmation dialog

**Estimated Effort**: 5-7 days

---

### 1.7 Frontend - React Hooks & State Management

**Status**: Not Started

**Custom Hooks**:
- [ ] `useQuotes()` - Fetch and manage quotes
- [ ] `useCustomers()` - Fetch and manage customers
- [ ] `useFoodItems()` - Fetch and manage items
- [ ] `useQuoteForm()` - Handle quote form state
- [ ] `useApiError()` - Handle and display API errors
- [ ] `useNotification()` - Toast notifications

**State Management** (MVP - Context API):
- [ ] AuthContext (placeholder for future auth)
- [ ] UIContext (notifications, loading states)
- [ ] Optional: Redux if needed later

**Estimated Effort**: 2-3 days

---

### 1.8 API Documentation & Testing

**Status**: Swagger configured, documentation needed

**OpenAPI Docs**:
- [ ] Document all endpoints
- [ ] Add example requests/responses
- [ ] Define error responses
- [ ] Add authentication scheme (JWT placeholder)

**Manual Testing**:
- [ ] Postman/Insomnia collection
- [ ] Test all endpoints
- [ ] Edge case validation

**Estimated Effort**: 1-2 days

---

## Phase 2: Email & Advanced Features

**Status**: Not Started (planned for 4-6 weeks after MVP)

### 2.1 Email Service Integration

- [ ] Email service abstraction (IEmailService)
- [ ] SendGrid/SMTP implementation
- [ ] Quote email template
- [ ] PDF attachment with quote
- [ ] Email delivery tracking
- [ ] Retry logic for failed sends
- [ ] Email logging

**Estimated Effort**: 3-4 days

### 2.2 PDF Generation

- [ ] PDF library integration (QuestPDF or iText)
- [ ] Quote PDF template
- [ ] Include pricing breakdown
- [ ] Include allergen/dietary info
- [ ] Email attachment
- [ ] Generate on demand endpoint

**Estimated Effort**: 2-3 days

### 2.3 Authentication & Authorization

- [ ] User entity and roles
- [ ] JWT token generation
- [ ] Login endpoint
- [ ] Protected endpoints
- [ ] Role-based access control
- [ ] Frontend login page

**Estimated Effort**: 3-4 days

### 2.4 Advanced Search & Filtering

- [ ] Quote search by number, customer, date range
- [ ] Customer search by name, email
- [ ] Food item search by name, allergens
- [ ] Saved filters
- [ ] Export functionality

**Estimated Effort**: 2-3 days

### 2.5 Reporting & Analytics

- [ ] Total quotes created
- [ ] Revenue by date range
- [ ] Quotes by customer
- [ ] Average quote value
- [ ] Margin analysis
- [ ] Rejection analysis

**Estimated Effort**: 2-3 days

---

## Phase 3: Performance & Reliability

**Status**: Planned (post-MVP)

### 3.1 Caching & Optimization

- [ ] Redis caching for reference data
- [ ] Quote caching strategy
- [ ] Query optimization
- [ ] Lazy loading
- [ ] Frontend bundle optimization

**Estimated Effort**: 2-3 days

### 3.2 Monitoring & Logging

- [ ] Application Insights setup
- [ ] Custom metrics
- [ ] Alert configuration
- [ ] Performance monitoring
- [ ] Error tracking

**Estimated Effort**: 2-3 days

### 3.3 Scaling & High Availability

- [ ] Database replication
- [ ] Load balancing
- [ ] Auto-scaling configuration
- [ ] Database connection pooling tuning
- [ ] Cache invalidation strategy

**Estimated Effort**: 3-4 days

---

## Phase 4: Security Hardening

**Status**: Planned (post-MVP)

- [ ] Input sanitization
- [ ] OWASP Top 10 hardening
- [ ] Rate limiting implementation
- [ ] DDoS protection
- [ ] Encryption at rest
- [ ] Encryption in transit
- [ ] Security audit
- [ ] Penetration testing

**Estimated Effort**: 4-5 days

---

## Implementation Workload Summary

### MVP Phase (Phase 1)

| Component | Estimated Days | Priority | Owner |
|-----------|---|----------|-------|
| Backend Endpoints | 3-4 | Critical | Backend Dev |
| Application Services | 4-5 | Critical | Backend Dev |
| Repositories | 2-3 | Critical | Backend Dev |
| Validation & Errors | 2-3 | High | Backend Dev |
| Backend Tests | 3-4 | High | Backend Dev |
| Frontend Pages | 5-7 | Critical | Frontend Dev |
| React Hooks | 2-3 | High | Frontend Dev |
| API Documentation | 1-2 | Medium | Backend Dev |
| **TOTAL MVP** | **22-31 days** | | **2 developers** |

### Timeline Estimate

**Assumption**: 2 developers working full-time

- **Week 1-2**: Backend endpoints + basic tests (~15 days)
- **Week 2-3**: Frontend pages + integration (~15 days)
- **Week 3**: Testing, bug fixes, polish (~5 days)

**Total**: 4-5 weeks to MVP launch

---

## Dependencies & Blockers

### External Dependencies
- None critical for MVP
- (Phase 2: Email provider, PDF library)

### Internal Dependencies

```
Domain Entities
    ↓
Application Services ← Validation Rules
    ↓
Repositories
    ↓
API Controllers ← DTOs
    ↓
Frontend Components ← API Client
```

**Critical Path**: Domain → Application → API → Frontend

---

## Definition of Done (MVP)

### Backend
- ✅ All endpoints implemented and documented
- ✅ Minimum 80% test coverage
- ✅ All code quality gates passing
- ✅ No high/critical security issues
- ✅ Error handling comprehensive
- ✅ Performance acceptable (< 500ms response time p95)

### Frontend
- ✅ All required pages implemented
- ✅ Mobile responsive design
- ✅ Form validation working
- ✅ Error handling & user feedback
- ✅ Accessible (WCAG AA minimum)
- ✅ Page load < 3s

### Deployment
- ✅ Docker images building successfully
- ✅ docker-compose working locally
- ✅ Database migrations applied
- ✅ CI/CD pipeline passing
- ✅ Documentation complete

### Testing
- ✅ Manual testing checklist completed
- ✅ No critical bugs found
- ✅ Performance tested
- ✅ Security scan passed

---

## Risk Management

### High Risk Items

| Risk | Impact | Mitigation |
|------|--------|-----------|
| Database performance | High | Early load testing, optimize queries |
| Complex pricing logic | Medium | Extensive unit tests, validation |
| Frontend responsiveness | Medium | Component testing, performance monitoring |
| API rate limiting not ready | Low | Can defer to Phase 2 |

### Mitigation Strategy
- Daily standups to catch blockers early
- Weekly code reviews
- Automated testing to catch regressions
- Staging deployment for final testing

---

## Success Criteria

### Technical Success
- ✅ All tests passing
- ✅ Code coverage > 80%
- ✅ CI/CD pipeline green
- ✅ No critical vulnerabilities
- ✅ Performance targets met

### Business Success
- ✅ Can create quotes
- ✅ Can calculate pricing correctly
- ✅ Can list/search quotes
- ✅ Can manage customers
- ✅ System is stable (99%+ uptime in testing)

### User Success
- ✅ Intuitive UI (minimal training needed)
- ✅ Fast response times
- ✅ Clear error messages
- ✅ Data persists correctly

---

## Rollout Plan

### Alpha (Internal Testing)
- Week 4: Deploy to staging
- Run manual testing checklist
- Identify and fix bugs

### Beta (Limited Users)
- Week 4-5: Deploy to production
- Limited user group tests
- Monitor for issues

### General Availability
- Week 5: Full launch
- Monitoring & support team ready
- Documentation finalized

---

## Post-Launch Monitoring

### Day 1-7
- Monitor error rates
- Check performance metrics
- Resolve critical issues immediately
- User feedback collection

### Week 2-4
- Optimize based on usage patterns
- Address feature requests
- Plan Phase 2 features
- Performance tuning

---

## Appendix: Development Environment

### Required Setup
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 16
- Docker Desktop
- VS Code or Visual Studio

### Commands Reference

```bash
# Backend
dotnet run --project AppHost              # Start all services
dotnet build
dotnet test
dotnet format
dotnet ef database update

# Frontend
npm install                                # One-time setup
npm run dev                                # Development
npm run build                              # Production build
npm run lint
npm run format

# Docker
docker-compose -f deploy/docker-compose.yml up -d
docker-compose -f deploy/docker-compose.yml logs -f

# Git
git status
git add .
git commit -m "message"
git push
```

### Testing Environments
- **Local**: Developer machine with Docker
- **Staging**: Pre-production test environment
- **Production**: Live environment

---

## Document History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-02-13 | Initial implementation plan based on specification v1.0 |

---

## Notes for Team

1. **Prioritize core functionality** over nice-to-haves for MVP
2. **Keep components reusable** - consider Phase 2 needs
3. **Test as you code** - don't wait for QA phase
4. **Document decisions** - add comments for complex logic
5. **Daily communication** - flag blockers immediately

---

**Next Review Date**: After Phase 1 completion
**Owner**: Development Team
**Last Updated**: 2026-02-13
