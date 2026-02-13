using CateringQuotes.Application.Repositories;
using CateringQuotes.Domain.Entities;
using CateringQuotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Infrastructure.Repositories;

/// <summary>
/// Repository for FoodItem data access.
/// </summary>
public class FoodItemRepository : IFoodItemRepository
{
    private readonly CateringQuotesDbContext _context;

    public FoodItemRepository(CateringQuotesDbContext context)
    {
        _context = context;
    }

    public async Task<FoodItem?> GetByIdAsync(int foodItemId)
    {
        return await _context.FoodItems.FindAsync(foodItemId);
    }

    public async Task<(List<FoodItem> Items, int Total)> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = _context.FoodItems;

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(f => f.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(List<FoodItem> Items, int Total)> GetActiveAsync(int page = 1, int pageSize = 20)
    {
        var query = _context.FoodItems.Where(f => f.IsActive);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(f => f.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<FoodItem> CreateAsync(FoodItem foodItem)
    {
        _context.FoodItems.Add(foodItem);
        await _context.SaveChangesAsync();
        return foodItem;
    }

    public async Task<FoodItem> UpdateAsync(FoodItem foodItem)
    {
        _context.FoodItems.Update(foodItem);
        await _context.SaveChangesAsync();
        return foodItem;
    }

    public async Task<bool> DeleteAsync(int foodItemId)
    {
        var foodItem = await _context.FoodItems.FindAsync(foodItemId);
        if (foodItem == null)
            return false;

        _context.FoodItems.Remove(foodItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> FoodItemExistsAsync(int foodItemId)
    {
        return await _context.FoodItems.AnyAsync(f => f.Id == foodItemId);
    }

    public async Task<List<FoodItem>> GetByIdsAsync(List<int> foodItemIds)
    {
        return await _context.FoodItems
            .Where(f => foodItemIds.Contains(f.Id))
            .ToListAsync();
    }
}
