using System.ComponentModel.DataAnnotations;

namespace CateringQuotes.Application.DTOs;

/// <summary>
/// DTO for creating a new food item.
/// </summary>
public class CreateFoodItemDto
{
    [Required(ErrorMessage = "Food item name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Food item name must be between 1 and 255 characters")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0")]
    public decimal CostPrice { get; set; }

    public int? Allergens { get; set; }
    public int? DietaryTags { get; set; }
}

/// <summary>
/// DTO for updating an existing food item.
/// </summary>
public class UpdateFoodItemDto
{
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Food item name must be between 1 and 255 characters")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0")]
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
