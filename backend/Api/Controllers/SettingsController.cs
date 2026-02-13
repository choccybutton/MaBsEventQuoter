using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Mappings;
using CateringQuotes.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace CateringQuotes.Api.Controllers;

/// <summary>
/// API endpoints for application settings.
/// </summary>
[ApiController]
[Route("api/v1/settings")]
[Produces("application/json")]
public class SettingsController : ControllerBase
{
    private readonly CateringQuotesDbContext _context;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(CateringQuotesDbContext context, ILogger<SettingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get application settings.
    /// </summary>
    /// <returns>Application settings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(AppSettingsDto), StatusCodes.Status200OK)]
    public ActionResult<AppSettingsDto> Get()
    {
        try
        {
            var settings = _context.AppSettings.Find(1);
            if (settings == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Application settings not found",
                    StatusCode = 404,
                });
            }

            return Ok(settings.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application settings");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving application settings",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Update application settings.
    /// </summary>
    /// <param name="dto">Updated settings data</param>
    /// <returns>Updated settings</returns>
    [HttpPut]
    [ProducesResponseType(typeof(AppSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<AppSettingsDto> Update([FromBody] UpdateAppSettingsDto dto)
    {
        try
        {
            // Validate input
            if (dto.DefaultVatRate.HasValue && (dto.DefaultVatRate < 0 || dto.DefaultVatRate > 1))
                return BadRequest(new ApiErrorResponse
                {
                    Message = "VAT rate must be between 0 and 1",
                    Errors = new Dictionary<string, string[]> { { "DefaultVatRate", new[] { "Must be between 0 and 1" } } },
                    StatusCode = 400,
                });

            if (dto.DefaultMarkupPercentage.HasValue && dto.DefaultMarkupPercentage < 0)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Markup percentage must be greater than or equal to 0",
                    Errors = new Dictionary<string, string[]> { { "DefaultMarkupPercentage", new[] { "Must be >= 0" } } },
                    StatusCode = 400,
                });

            var settings = _context.AppSettings.Find(1);
            if (settings == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "Application settings not found",
                    StatusCode = 404,
                });
            }

            dto.UpdateEntity(settings);
            _context.AppSettings.Update(settings);
            _context.SaveChanges();

            _logger.LogInformation("Application settings updated");

            return Ok(settings.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application settings");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while updating application settings",
                StatusCode = 500,
            });
        }
    }
}
