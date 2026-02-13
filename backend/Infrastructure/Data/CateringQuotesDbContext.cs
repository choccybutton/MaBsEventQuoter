using CateringQuotes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Infrastructure.Data;

public class CateringQuotesDbContext : DbContext
{
    public CateringQuotesDbContext(DbContextOptions<CateringQuotesDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteLineItem> QuoteLineItems => Set<QuoteLineItem>();
    public DbSet<Allergen> Allergens => Set<Allergen>();
    public DbSet<DietaryTag> DietaryTags => Set<DietaryTag>();
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Company).HasMaxLength(200);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure FoodItem
        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CostPrice).HasPrecision(10, 2);
        });

        // Configure Quote
        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuoteNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.VatRate).HasPrecision(5, 2);
            entity.Property(e => e.TotalCost).HasPrecision(10, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(10, 2);
            entity.Property(e => e.Margin).HasPrecision(5, 4);
            entity.Property(e => e.MarkupPercentage).HasPrecision(5, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            // Foreign key
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Quotes)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            entity.HasIndex(e => e.QuoteNumber).IsUnique();
            entity.HasIndex(e => e.Status);
        });

        // Configure QuoteLineItem
        modelBuilder.Entity<QuoteLineItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.UnitCost).HasPrecision(10, 2);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Property(e => e.LineTotal).HasPrecision(10, 2);

            // Foreign keys
            entity.HasOne(e => e.Quote)
                .WithMany(q => q.LineItems)
                .HasForeignKey(e => e.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FoodItem)
                .WithMany(f => f.QuoteLineItems)
                .HasForeignKey(e => e.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Allergen
        modelBuilder.Entity<Allergen>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure DietaryTag
        modelBuilder.Entity<DietaryTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Configure AppSettings
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DefaultVatRate).HasPrecision(5, 2);
            entity.Property(e => e.DefaultMarkupPercentage).HasPrecision(5, 2);
            entity.Property(e => e.MarginGreenThresholdPct).HasPrecision(5, 2);
            entity.Property(e => e.MarginAmberThresholdPct).HasPrecision(5, 2);

            // Ensure only one row
            entity.HasData(new AppSettings { Id = 1 });
        });
    }
}
