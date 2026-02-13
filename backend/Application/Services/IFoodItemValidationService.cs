using CateringQuotes.Application.DTOs;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for validating food item-related inputs.
/// </summary>
public interface IFoodItemValidationService
{
    /// <summary>
    /// Validate create food item DTO.
    /// Throws ValidationException if validation fails.
    /// </summary>
    void ValidateCreateFoodItemDto(CreateFoodItemDto dto);

    /// <summary>
    /// Validate update food item DTO.
    /// Throws ValidationException if validation fails.
    /// </summary>
    void ValidateUpdateFoodItemDto(UpdateFoodItemDto dto);
}
