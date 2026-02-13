using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Mappings;
using CateringQuotes.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Api.Controllers;

/// <summary>
/// API endpoints for reference data (allergens, dietary tags).
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
public class ReferenceDataController : ControllerBase
{
    private readonly CateringQuotesDbContext _context;
    private readonly ILogger<ReferenceDataController> _logger;

    public ReferenceDataController(CateringQuotesDbContext context, ILogger<ReferenceDataController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all allergens.
    /// </summary>
    /// <returns>List of allergens</returns>
    [HttpGet("allergens")]
    [ProducesResponseType(typeof(List<AllergenDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AllergenDto>>> GetAllergens()
    {
        try
        {
            var allergens = await _context.Allergens
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return Ok(allergens.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting allergens");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving allergens",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Get all dietary tags.
    /// </summary>
    /// <returns>List of dietary tags</returns>
    [HttpGet("dietary-tags")]
    [ProducesResponseType(typeof(List<DietaryTagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DietaryTagDto>>> GetDietaryTags()
    {
        try
        {
            var tags = await _context.DietaryTags
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Ok(tags.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dietary tags");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving dietary tags",
                StatusCode = 500,
            });
        }
    }
}
