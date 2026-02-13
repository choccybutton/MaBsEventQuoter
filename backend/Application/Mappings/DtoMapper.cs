using CateringQuotes.Application.DTOs;
using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Mappings;

/// <summary>
/// Maps between domain entities and DTOs.
/// </summary>
public static class DtoMapper
{
    // Customer mappings
    public static CustomerDto ToDto(this Customer customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Email = customer.Email,
        Phone = customer.Phone,
        Company = customer.Company,
        CreatedAt = customer.CreatedAt,
        ModifiedAt = customer.ModifiedAt,
    };

    public static List<CustomerDto> ToDto(this IEnumerable<Customer> customers) =>
        customers.Select(ToDto).ToList();

    public static Customer ToEntity(this CreateCustomerDto dto) => new()
    {
        Name = dto.Name,
        Email = dto.Email,
        Phone = dto.Phone,
        Company = dto.Company,
    };

    public static void UpdateEntity(this UpdateCustomerDto dto, Customer entity)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Email)) entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.Company = dto.Company;
        entity.ModifiedAt = DateTime.UtcNow;
    }

    // Quote mappings
    public static QuoteDto ToDto(this Quote quote) => new()
    {
        Id = quote.Id,
        CustomerId = quote.CustomerId,
        Customer = quote.Customer?.ToDto(),
        QuoteNumber = quote.QuoteNumber,
        QuoteDate = quote.QuoteDate,
        EventDate = quote.EventDate,
        Status = quote.Status,
        VatRate = quote.VatRate,
        TotalCost = quote.TotalCost,
        TotalPrice = quote.TotalPrice,
        Margin = quote.Margin,
        MarkupPercentage = quote.MarkupPercentage,
        Notes = quote.Notes,
        CreatedAt = quote.CreatedAt,
        ModifiedAt = quote.ModifiedAt,
        SentAt = quote.SentAt,
        LineItems = quote.LineItems.ToDto(),
    };

    public static List<QuoteDto> ToDto(this IEnumerable<Quote> quotes) =>
        quotes.Select(ToDto).ToList();

    public static QuoteLineItemDto ToDto(this QuoteLineItem item) => new()
    {
        Id = item.Id,
        QuoteId = item.QuoteId,
        FoodItemId = item.FoodItemId,
        Description = item.Description,
        Quantity = item.Quantity,
        UnitCost = item.UnitCost,
        UnitPrice = item.UnitPrice,
        LineTotal = item.LineTotal,
        DisplayOrder = item.DisplayOrder,
    };

    public static List<QuoteLineItemDto> ToDto(this IEnumerable<QuoteLineItem> items) =>
        items.Select(ToDto).ToList();

    // FoodItem mappings
    public static FoodItemDto ToDto(this FoodItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        CostPrice = item.CostPrice,
        Allergens = item.Allergens,
        DietaryTags = item.DietaryTags,
        IsActive = item.IsActive,
        CreatedAt = item.CreatedAt,
        ModifiedAt = item.ModifiedAt,
    };

    public static List<FoodItemDto> ToDto(this IEnumerable<FoodItem> items) =>
        items.Select(ToDto).ToList();

    public static FoodItem ToEntity(this CreateFoodItemDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        CostPrice = dto.CostPrice,
        Allergens = dto.Allergens,
        DietaryTags = dto.DietaryTags,
        IsActive = true,
    };

    public static void UpdateEntity(this UpdateFoodItemDto dto, FoodItem entity)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Description)) entity.Description = dto.Description;
        if (dto.CostPrice.HasValue) entity.CostPrice = dto.CostPrice.Value;
        if (dto.Allergens.HasValue) entity.Allergens = dto.Allergens;
        if (dto.DietaryTags.HasValue) entity.DietaryTags = dto.DietaryTags;
        if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
        entity.ModifiedAt = DateTime.UtcNow;
    }

    // Reference data mappings
    public static AllergenDto ToDto(this Allergen allergen) => new()
    {
        Id = allergen.Id,
        Code = allergen.Code,
        Name = allergen.Name,
        Description = allergen.Description,
        IsActive = allergen.IsActive,
    };

    public static List<AllergenDto> ToDto(this IEnumerable<Allergen> allergens) =>
        allergens.Select(ToDto).ToList();

    public static DietaryTagDto ToDto(this DietaryTag tag) => new()
    {
        Id = tag.Id,
        Code = tag.Code,
        Name = tag.Name,
        Description = tag.Description,
        IsActive = tag.IsActive,
    };

    public static List<DietaryTagDto> ToDto(this IEnumerable<DietaryTag> tags) =>
        tags.Select(ToDto).ToList();

    public static AppSettingsDto ToDto(this AppSettings settings) => new()
    {
        Id = settings.Id,
        DefaultVatRate = settings.DefaultVatRate,
        DefaultMarkupPercentage = settings.DefaultMarkupPercentage,
        MarginGreenThresholdPct = settings.MarginGreenThresholdPct,
        MarginAmberThresholdPct = settings.MarginAmberThresholdPct,
        CreatedAt = settings.CreatedAt,
        ModifiedAt = settings.ModifiedAt,
    };

    public static void UpdateEntity(this UpdateAppSettingsDto dto, AppSettings entity)
    {
        if (dto.DefaultVatRate.HasValue) entity.DefaultVatRate = dto.DefaultVatRate.Value;
        if (dto.DefaultMarkupPercentage.HasValue) entity.DefaultMarkupPercentage = dto.DefaultMarkupPercentage.Value;
        if (dto.MarginGreenThresholdPct.HasValue) entity.MarginGreenThresholdPct = dto.MarginGreenThresholdPct.Value;
        if (dto.MarginAmberThresholdPct.HasValue) entity.MarginAmberThresholdPct = dto.MarginAmberThresholdPct.Value;
        entity.ModifiedAt = DateTime.UtcNow;
    }
}
