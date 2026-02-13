# Database Design

## Overview

PostgreSQL database for the Catering Quotes application, configured with Entity Framework Core.

## Entities

### Core Entities

#### Customer
- **Purpose**: Stores customer information
- **Key Fields**: Name, Email, Phone, Company
- **Relationships**: One-to-Many with Quote
- **Constraints**: Email is unique

#### Quote
- **Purpose**: Main entity for catering quotes
- **Key Fields**: QuoteNumber (unique), Status, VatRate, Markup, Margin, TotalCost, TotalPrice
- **Relationships**:
  - Many-to-One with Customer
  - One-to-Many with QuoteLineItem
- **Cascade Delete**: Deletes related LineItems when quote is deleted

#### QuoteLineItem
- **Purpose**: Line items within a quote (individual food items)
- **Key Fields**: Quantity, UnitCost, UnitPrice, LineTotal, DisplayOrder
- **Relationships**:
  - Many-to-One with Quote
  - Many-to-One with FoodItem

#### FoodItem
- **Purpose**: Reusable food items that can be added to quotes
- **Key Fields**: Name, Description, CostPrice, IsActive
- **Relationships**: One-to-Many with QuoteLineItem

### Lookup Tables (Reference Data)

#### Allergen
- **Purpose**: Define allergen types (14 UK allergens)
- **Fields**: Code (unique), Name, Description, IsActive
- **Examples**: CELERY, NUTS, PEANUTS, MILK, EGGS, etc.

#### DietaryTag
- **Purpose**: Define dietary requirements
- **Fields**: Code (unique), Name, Description, IsActive
- **Examples**: VEGAN, VEGETARIAN, GLUTEN_FREE, DAIRY_FREE, HALAL, KOSHER, NUT_FREE

### Configuration Table

#### AppSettings
- **Purpose**: Application-wide settings (single row)
- **Fields**:
  - DefaultVatRate (20%)
  - DefaultMarkupPercentage (70%)
  - MarginGreenThresholdPct (70%)
  - MarginAmberThresholdPct (60%)

## Database Schema

### Key Constraints

- **Unique Constraints**: Customer.Email, Quote.QuoteNumber, Allergen.Code, DietaryTag.Code
- **Foreign Keys**: All relationships enforced with appropriate cascade behaviors
- **Default Values**: Timestamps set to UTC now
- **Precision**: Decimal fields use precision(10,2) for currency, (5,2) for percentages

### Indexes

- Quote.Status (for filtering)
- Customer.Email (for uniqueness)
- Allergen.Code (for uniqueness)
- DietaryTag.Code (for uniqueness)
- Quote.QuoteNumber (for uniqueness)

## Migrations

### InitialCreate
- Creates all tables with proper constraints
- Seeds default AppSettings row
- Version: 20260213024636

## Database Seeding

### Automatic Seeding

On application startup (Development environment):
1. Migrations are applied
2. Reference data is seeded:
   - 14 allergens (UK standard)
   - 7 dietary tags
   - App settings (defaults)

### Demo Data

Optional demo data includes:
- 5 sample customers
- 10 sample food items

Activated via `POST /api/system/reset-db?seedDemo=true`

## Development Commands

### Apply Migrations
```bash
dotnet ef database update --project backend/Infrastructure --startup-project backend/Api
```

### Create New Migration
```bash
dotnet ef migrations add MigrationName --project backend/Infrastructure --startup-project backend/Api
```

### Reset Database (Development Only)
```bash
# Via API
curl -X POST http://localhost:5000/api/system/reset-db

# With demo data
curl -X POST "http://localhost:5000/api/system/reset-db?seedDemo=true"
```

### View Connection String
- Development: `Host=localhost;Port=5432;Database=catering_quotes;Username=postgres;Password=postgres;`
- Configured in: `appsettings.json` or Aspire connection
- Via Aspire: `dotnet run --project AppHost` (automatic)

## Data Integrity

### Idempotency
- All seed operations check if data exists before inserting
- Safe to run multiple times

### Cascade Rules
- Quote deletion → automatically deletes LineItems
- FoodItem deletion → restricted (prevents orphaning line items)
- Customer deletion → cascades to quotes and line items

## Backup Strategy

Per specification:
- Automated daily backups required
- Minimum 14-day retention (dev), 30+ days (production)
- Backup restore testing monthly

## Performance Considerations

- Connection pooling configured (Min: 5, Max: 20)
- Proper indexes on frequently queried fields
- Precision configured for decimal accuracy in financial calculations
- FK relationships optimized with DeleteBehavior settings
