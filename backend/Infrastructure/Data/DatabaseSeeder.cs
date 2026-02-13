using CateringQuotes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Infrastructure.Data;

/// <summary>
/// Seeds the database with reference data (allergens, dietary tags, settings).
/// </summary>
public class DatabaseSeeder
{
    private readonly CateringQuotesDbContext _context;

    public DatabaseSeeder(CateringQuotesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds all required reference data idempotently.
    /// </summary>
    public async Task SeedAsync()
    {
        await SeedAllergensAsync();
        await SeedDietaryTagsAsync();
        await SeedAppSettingsAsync();
    }

    /// <summary>
    /// Clears all data and reseeds (development only).
    /// </summary>
    public async Task ResetAndSeedAsync(bool includeDemoData = false)
    {
        // Delete all data
        _context.QuoteLineItems.RemoveRange(_context.QuoteLineItems);
        _context.Quotes.RemoveRange(_context.Quotes);
        _context.FoodItems.RemoveRange(_context.FoodItems);
        _context.Customers.RemoveRange(_context.Customers);
        _context.Allergens.RemoveRange(_context.Allergens);
        _context.DietaryTags.RemoveRange(_context.DietaryTags);

        await _context.SaveChangesAsync();

        // Reseed reference data
        await SeedAsync();

        // Seed demo data if requested
        if (includeDemoData)
        {
            await SeedDemoDataAsync();
        }
    }

    private async Task SeedAllergensAsync()
    {
        if (await _context.Allergens.AnyAsync())
            return;

        var allergens = new[]
        {
            new Allergen { Code = "CELERY", Name = "Celery" },
            new Allergen { Code = "CEREALS_GLUTEN", Name = "Cereals containing gluten" },
            new Allergen { Code = "CRUSTACEANS", Name = "Crustaceans" },
            new Allergen { Code = "EGGS", Name = "Eggs" },
            new Allergen { Code = "FISH", Name = "Fish" },
            new Allergen { Code = "LUPIN", Name = "Lupin" },
            new Allergen { Code = "MILK", Name = "Milk" },
            new Allergen { Code = "MOLLUSCS", Name = "Molluscs" },
            new Allergen { Code = "MUSTARD", Name = "Mustard" },
            new Allergen { Code = "NUTS", Name = "Tree nuts" },
            new Allergen { Code = "PEANUTS", Name = "Peanuts" },
            new Allergen { Code = "SESAME", Name = "Sesame" },
            new Allergen { Code = "SOYA", Name = "Soya" },
            new Allergen { Code = "SULPHITES", Name = "Sulphites" }
        };

        _context.Allergens.AddRange(allergens);
        await _context.SaveChangesAsync();
    }

    private async Task SeedDietaryTagsAsync()
    {
        if (await _context.DietaryTags.AnyAsync())
            return;

        var dietaryTags = new[]
        {
            new DietaryTag { Code = "VEGAN", Name = "Vegan" },
            new DietaryTag { Code = "VEGETARIAN", Name = "Vegetarian" },
            new DietaryTag { Code = "GLUTEN_FREE", Name = "Gluten Free" },
            new DietaryTag { Code = "DAIRY_FREE", Name = "Dairy Free" },
            new DietaryTag { Code = "NUT_FREE", Name = "Nut Free" },
            new DietaryTag { Code = "HALAL", Name = "Halal" },
            new DietaryTag { Code = "KOSHER", Name = "Kosher" }
        };

        _context.DietaryTags.AddRange(dietaryTags);
        await _context.SaveChangesAsync();
    }

    private async Task SeedAppSettingsAsync()
    {
        if (await _context.AppSettings.AnyAsync())
            return;

        var settings = new AppSettings
        {
            Id = 1,
            DefaultVatRate = 0.20m,
            DefaultMarkupPercentage = 0.70m,
            MarginGreenThresholdPct = 0.70m,
            MarginAmberThresholdPct = 0.60m
        };

        _context.AppSettings.Add(settings);
        await _context.SaveChangesAsync();
    }

    private async Task SeedDemoDataAsync()
    {
        // Demo customers
        var customers = new[]
        {
            new Customer { Name = "Acme Corporation", Email = "contact@acme.com", Company = "Acme Inc." },
            new Customer { Name = "Tech Startup Ltd", Email = "events@techstartup.com", Company = "Tech Startup" },
            new Customer { Name = "Global Industries", Email = "catering@global.com", Company = "Global Industries" },
            new Customer { Name = "Local Charity", Email = "fundraiser@localcharity.org" },
            new Customer { Name = "University Events", Email = "events@university.edu" }
        };

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        // Demo food items
        var foodItems = new[]
        {
            new FoodItem { Name = "Caesar Salad", Description = "Fresh romaine with parmesan and croutons", CostPrice = 3.50m },
            new FoodItem { Name = "Grilled Chicken", Description = "Herb-marinated chicken breast", CostPrice = 5.00m },
            new FoodItem { Name = "Pasta Carbonara", Description = "Classic Italian pasta with bacon", CostPrice = 4.00m },
            new FoodItem { Name = "Vegetable Stir Fry", Description = "Mixed seasonal vegetables", CostPrice = 3.00m },
            new FoodItem { Name = "Chocolate Dessert", Description = "Rich chocolate mousse", CostPrice = 2.50m },
            new FoodItem { Name = "Fruit Platter", Description = "Seasonal fresh fruits", CostPrice = 2.00m },
            new FoodItem { Name = "Bread Basket", Description = "Assorted fresh breads", CostPrice = 1.50m },
            new FoodItem { Name = "Coffee & Tea", Description = "Selection of hot beverages", CostPrice = 0.75m },
            new FoodItem { Name = "Vegetarian Wrap", Description = "Hummus and vegetables in wrap", CostPrice = 2.75m },
            new FoodItem { Name = "Seafood Platter", Description = "Mixed fresh seafood", CostPrice = 7.00m }
        };

        _context.FoodItems.AddRange(foodItems);
        await _context.SaveChangesAsync();
    }
}
