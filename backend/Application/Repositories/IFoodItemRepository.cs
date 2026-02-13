using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Repositories;

/// <summary>
/// Repository interface for FoodItem data access.
/// </summary>
public interface IFoodItemRepository
{
    /// <summary>
    /// Get food item by ID.
    /// </summary>
    Task<FoodItem?> GetByIdAsync(int foodItemId);

    /// <summary>
    /// Get all food items with pagination.
    /// </summary>
    Task<(List<FoodItem> Items, int Total)> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Get only active food items with pagination.
    /// </summary>
    Task<(List<FoodItem> Items, int Total)> GetActiveAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Create a new food item.
    /// </summary>
    Task<FoodItem> CreateAsync(FoodItem foodItem);

    /// <summary>
    /// Update an existing food item.
    /// </summary>
    Task<FoodItem> UpdateAsync(FoodItem foodItem);

    /// <summary>
    /// Delete a food item by ID.
    /// </summary>
    Task<bool> DeleteAsync(int foodItemId);

    /// <summary>
    /// Check if a food item exists by ID.
    /// </summary>
    Task<bool> FoodItemExistsAsync(int foodItemId);

    /// <summary>
    /// Get food items by IDs (for bulk operations).
    /// </summary>
    Task<List<FoodItem>> GetByIdsAsync(List<int> foodItemIds);
}
