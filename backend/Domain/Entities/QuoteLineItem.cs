namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Represents a line item in a quote (a specific food item).
/// </summary>
public class QuoteLineItem
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public int FoodItemId { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation
    public Quote Quote { get; set; } = null!;
    public FoodItem FoodItem { get; set; } = null!;
}
