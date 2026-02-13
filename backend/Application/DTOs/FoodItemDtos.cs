namespace CateringQuotes.Application.DTOs;

/// <summary>
/// DTO for creating a new food item.
/// </summary>
public class CreateFoodItemDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public int? Allergens { get; set; }
    public int? DietaryTags { get; set; }
}

/// <summary>
/// DTO for updating an existing food item.
/// </summary>
public class UpdateFoodItemDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? CostPrice { get; set; }
    public int? Allergens { get; set; }
    public int? DietaryTags { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// DTO for displaying food item information.
/// </summary>
public class FoodItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal CostPrice { get; set; }
    public int? Allergens { get; set; }
    public int? DietaryTags { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
