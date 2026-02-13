using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Mappings;
using CateringQuotes.Domain.Entities;
using CateringQuotes.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Api.Controllers;

/// <summary>
/// API endpoints for managing food items.
/// </summary>
[ApiController]
[Route("api/v1/food-items")]
[Produces("application/json")]
public class FoodItemsController : ControllerBase
{
    private readonly CateringQuotesDbContext _context;
    private readonly ILogger<FoodItemsController> _logger;

    public FoodItemsController(CateringQuotesDbContext context, ILogger<FoodItemsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all food items with optional filtering.
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="activeOnly">Only return active items</param>
    /// <returns>Paginated list of food items</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<FoodItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<FoodItemDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool activeOnly = false)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _context.FoodItems.AsQueryable();

            if (activeOnly)
                query = query.Where(f => f.IsActive);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(f => f.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PaginatedResponse<FoodItemDto>
            {
                Items = items.ToDto(),
                Total = total,
                Page = page,
                PageSize = pageSize,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting food items");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving food items",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Get a specific food item by ID.
    /// </summary>
    /// <param name="id">Food item ID</param>
    /// <returns>Food item details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FoodItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodItemDto>> GetById(int id)
    {
        try
        {
            var item = await _context.FoodItems.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Food item with ID {id} not found",
                    StatusCode = 404,
                });
            }

            return Ok(item.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting food item {FoodItemId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving the food item",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Create a new food item.
    /// </summary>
    /// <param name="dto">Food item data</param>
    /// <returns>Created food item</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FoodItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FoodItemDto>> Create([FromBody] CreateFoodItemDto dto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Food item name is required",
                    StatusCode = 400,
                });

            if (dto.CostPrice <= 0)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Cost price must be greater than 0",
                    StatusCode = 400,
                });

            var item = dto.ToEntity();
            _context.FoodItems.Add(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Food item created: {FoodItemId} - {FoodItemName}", item.Id, item.Name);

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating food item");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while creating the food item",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Update an existing food item.
    /// </summary>
    /// <param name="id">Food item ID</param>
    /// <param name="dto">Updated food item data</param>
    /// <returns>Updated food item</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FoodItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FoodItemDto>> Update(int id, [FromBody] UpdateFoodItemDto dto)
    {
        try
        {
            var item = await _context.FoodItems.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Food item with ID {id} not found",
                    StatusCode = 404,
                });
            }

            if (dto.CostPrice.HasValue && dto.CostPrice <= 0)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Cost price must be greater than 0",
                    StatusCode = 400,
                });

            dto.UpdateEntity(item);
            _context.FoodItems.Update(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Food item updated: {FoodItemId}", id);

            return Ok(item.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating food item {FoodItemId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while updating the food item",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Delete a food item.
    /// </summary>
    /// <param name="id">Food item ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _context.FoodItems.FindAsync(id);
            if (item == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Food item with ID {id} not found",
                    StatusCode = 404,
                });
            }

            _context.FoodItems.Remove(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Food item deleted: {FoodItemId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting food item {FoodItemId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while deleting the food item",
                StatusCode = 500,
            });
        }
    }
}
