namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Represents a food item that can be added to catering quotes.
/// </summary>
public class FoodItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public int? Allergens { get; set; } // Bitmask or reference
    public int? DietaryTags { get; set; } // Bitmask or reference
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    // Navigation
    public ICollection<QuoteLineItem> QuoteLineItems { get; set; } = new List<QuoteLineItem>();
}
