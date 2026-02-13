using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Mappings;
using CateringQuotes.Domain.Entities;
using CateringQuotes.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Api.Controllers;

/// <summary>
/// API endpoints for managing catering quotes.
/// </summary>
[ApiController]
[Route("api/v1/quotes")]
[Produces("application/json")]
public class QuotesController : ControllerBase
{
    private readonly CateringQuotesDbContext _context;
    private readonly ILogger<QuotesController> _logger;

    public QuotesController(CateringQuotesDbContext context, ILogger<QuotesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all quotes with optional filtering and pagination.
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="status">Filter by status</param>
    /// <param name="customerId">Filter by customer ID</param>
    /// <returns>Paginated list of quotes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<QuoteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<QuoteDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] int? customerId = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.LineItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(q => q.Status == status);

            if (customerId.HasValue)
                query = query.Where(q => q.CustomerId == customerId);

            var total = await query.CountAsync();
            var quotes = await query
                .OrderByDescending(q => q.QuoteDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PaginatedResponse<QuoteDto>
            {
                Items = quotes.ToDto(),
                Total = total,
                Page = page,
                PageSize = pageSize,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quotes");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving quotes",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Get a specific quote by ID.
    /// </summary>
    /// <param name="id">Quote ID</param>
    /// <returns>Quote details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuoteDto>> GetById(int id)
    {
        try
        {
            var quote = await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.LineItems)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Quote with ID {id} not found",
                    StatusCode = 404,
                });
            }

            return Ok(quote.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quote {QuoteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving the quote",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Create a new quote.
    /// </summary>
    /// <param name="dto">Quote data</param>
    /// <returns>Created quote</returns>
    [HttpPost]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuoteDto>> Create([FromBody] CreateQuoteDto dto)
    {
        try
        {
            // Validate input
            if (dto.CustomerId <= 0)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Valid customer ID is required",
                    StatusCode = 400,
                });

            if (dto.LineItems.Count == 0)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "At least one line item is required",
                    StatusCode = 400,
                });

            // Verify customer exists
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                return BadRequest(new ApiErrorResponse
                {
                    Message = $"Customer with ID {dto.CustomerId} not found",
                    StatusCode = 400,
                });

            // Get settings for defaults
            var settings = await _context.AppSettings.FindAsync(1);
            var vatRate = dto.VatRate ?? settings?.DefaultVatRate ?? 0.20m;
            var markupPercentage = dto.MarkupPercentage ?? settings?.DefaultMarkupPercentage ?? 0.70m;

            // Create quote
            var quote = new Quote
            {
                CustomerId = dto.CustomerId,
                Customer = customer,
                QuoteNumber = GenerateQuoteNumber(),
                Status = "Draft",
                VatRate = vatRate,
                MarkupPercentage = markupPercentage,
                EventDate = dto.EventDate,
                Notes = dto.Notes,
                LineItems = new List<QuoteLineItem>(),
            };

            // Add line items and calculate totals
            decimal totalCost = 0;
            int displayOrder = 1;

            foreach (var item in dto.LineItems)
            {
                var foodItem = await _context.FoodItems.FindAsync(item.FoodItemId);
                if (foodItem == null)
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = $"Food item with ID {item.FoodItemId} not found",
                        StatusCode = 400,
                    });

                if (item.Quantity <= 0)
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Quantity must be greater than 0",
                        StatusCode = 400,
                    });

                var lineCost = item.UnitCost * item.Quantity;
                var linePrice = lineCost * (1 + markupPercentage);

                quote.LineItems.Add(new QuoteLineItem
                {
                    FoodItemId = item.FoodItemId,
                    FoodItem = foodItem,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    UnitPrice = linePrice / item.Quantity,
                    LineTotal = linePrice,
                    DisplayOrder = displayOrder++,
                });

                totalCost += lineCost;
            }

            // Calculate totals
            var priceBeforeVat = totalCost * (1 + markupPercentage);
            var vat = priceBeforeVat * vatRate;
            var totalPrice = priceBeforeVat + vat;
            var margin = (totalPrice - totalCost) / totalPrice;

            quote.TotalCost = totalCost;
            quote.TotalPrice = totalPrice;
            quote.Margin = margin;

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quote created: {QuoteId} - {QuoteNumber}", quote.Id, quote.QuoteNumber);

            return CreatedAtAction(nameof(GetById), new { id = quote.Id }, quote.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quote");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while creating the quote",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Update an existing quote.
    /// </summary>
    /// <param name="id">Quote ID</param>
    /// <param name="dto">Updated quote data</param>
    /// <returns>Updated quote</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuoteDto>> Update(int id, [FromBody] UpdateQuoteDto dto)
    {
        try
        {
            var quote = await _context.Quotes
                .Include(q => q.Customer)
                .Include(q => q.LineItems)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Quote with ID {id} not found",
                    StatusCode = 404,
                });
            }

            // Only allow updates to Draft quotes
            if (quote.Status != "Draft")
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Only Draft quotes can be updated",
                    StatusCode = 400,
                });

            // Update basic fields
            quote.EventDate = dto.EventDate ?? quote.EventDate;
            quote.Notes = dto.Notes ?? quote.Notes;

            if (dto.VatRate.HasValue)
                quote.VatRate = dto.VatRate.Value;

            if (dto.MarkupPercentage.HasValue)
                quote.MarkupPercentage = dto.MarkupPercentage.Value;

            // Update line items if provided
            if (dto.LineItems != null && dto.LineItems.Count > 0)
            {
                if (dto.LineItems.Count == 0)
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "At least one line item is required",
                        StatusCode = 400,
                    });

                quote.LineItems.Clear();

                decimal totalCost = 0;
                int displayOrder = 1;

                foreach (var item in dto.LineItems)
                {
                    var foodItem = await _context.FoodItems.FindAsync(item.FoodItemId);
                    if (foodItem == null)
                        return BadRequest(new ApiErrorResponse
                        {
                            Message = $"Food item with ID {item.FoodItemId} not found",
                            StatusCode = 400,
                        });

                    if (item.Quantity <= 0)
                        return BadRequest(new ApiErrorResponse
                        {
                            Message = "Quantity must be greater than 0",
                            StatusCode = 400,
                        });

                    var lineCost = item.UnitCost * item.Quantity;
                    var linePrice = lineCost * (1 + quote.MarkupPercentage);

                    quote.LineItems.Add(new QuoteLineItem
                    {
                        FoodItemId = item.FoodItemId,
                        FoodItem = foodItem,
                        Description = item.Description,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost,
                        UnitPrice = linePrice / item.Quantity,
                        LineTotal = linePrice,
                        DisplayOrder = displayOrder++,
                    });

                    totalCost += lineCost;
                }

                // Recalculate totals
                var priceBeforeVat = totalCost * (1 + quote.MarkupPercentage);
                var vat = priceBeforeVat * quote.VatRate;
                var totalPrice = priceBeforeVat + vat;
                var margin = (totalPrice - totalCost) / totalPrice;

                quote.TotalCost = totalCost;
                quote.TotalPrice = totalPrice;
                quote.Margin = margin;
            }

            quote.ModifiedAt = DateTime.UtcNow;
            _context.Quotes.Update(quote);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quote updated: {QuoteId}", id);

            return Ok(quote.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quote {QuoteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while updating the quote",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Delete a quote.
    /// </summary>
    /// <param name="id">Quote ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Quote with ID {id} not found",
                    StatusCode = 404,
                });
            }

            // Only allow deletion of Draft quotes
            if (quote.Status != "Draft")
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Only Draft quotes can be deleted",
                    StatusCode = 400,
                });

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quote deleted: {QuoteId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quote {QuoteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while deleting the quote",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Generate a unique quote number.
    /// Format: QT-2026-001 where 2026 is year and 001 is sequence.
    /// </summary>
    private string GenerateQuoteNumber()
    {
        var year = DateTime.UtcNow.Year;
        var count = _context.Quotes.Count(q => q.QuoteNumber.StartsWith($"QT-{year}")) + 1;
        return $"QT-{year}-{count:D3}";
    }
}
