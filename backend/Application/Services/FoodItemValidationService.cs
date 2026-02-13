using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Exceptions;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for validating food item-related inputs.
/// </summary>
public class FoodItemValidationService : IFoodItemValidationService
{
    public void ValidateCreateFoodItemDto(CreateFoodItemDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validate name
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Name", new[] { "Food item name is required" });

        // Validate cost price
        if (dto.CostPrice <= 0)
            errors.Add("CostPrice", new[] { "Cost price must be greater than 0" });

        if (errors.Count > 0)
            throw new ValidationException("Food item validation failed", errors);
    }

    public void ValidateUpdateFoodItemDto(UpdateFoodItemDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validate name if provided
        if (!string.IsNullOrWhiteSpace(dto.Name) && string.IsNullOrWhiteSpace(dto.Name.Trim()))
            errors.Add("Name", new[] { "Food item name cannot be empty" });

        // Validate cost price if provided
        if (dto.CostPrice.HasValue && dto.CostPrice <= 0)
            errors.Add("CostPrice", new[] { "Cost price must be greater than 0" });

        if (errors.Count > 0)
            throw new ValidationException("Food item validation failed", errors);
    }
}
