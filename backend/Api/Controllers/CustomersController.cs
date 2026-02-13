using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Mappings;
using CateringQuotes.Domain.Entities;
using CateringQuotes.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Api.Controllers;

/// <summary>
/// API endpoints for managing customers.
/// </summary>
[ApiController]
[Route("api/v1/customers")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly CateringQuotesDbContext _context;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(CateringQuotesDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers with pagination.
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of customers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<CustomerDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var total = await _context.Customers.CountAsync();
            var customers = await _context.Customers
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PaginatedResponse<CustomerDto>
            {
                Items = customers.ToDto(),
                Total = total,
                Page = page,
                PageSize = pageSize,
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customers");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving customers",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Get a specific customer by ID.
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Customer with ID {id} not found",
                    StatusCode = 404,
                });
            }

            return Ok(customer.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while retrieving the customer",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Create a new customer.
    /// </summary>
    /// <param name="dto">Customer data</param>
    /// <returns>Created customer</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Customer name is required",
                    StatusCode = 400,
                });

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Customer email is required",
                    StatusCode = 400,
                });

            // Check for duplicate email
            var exists = await _context.Customers.AnyAsync(c => c.Email == dto.Email);
            if (exists)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "A customer with this email already exists",
                    Errors = new Dictionary<string, string[]> { { "Email", new[] { "Email already in use" } } },
                    StatusCode = 400,
                });

            var customer = dto.ToEntity();
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer created: {CustomerId} - {CustomerName}", customer.Id, customer.Name);

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while creating the customer",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Update an existing customer.
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="dto">Updated customer data</param>
    /// <returns>Updated customer</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Customer with ID {id} not found",
                    StatusCode = 404,
                });
            }

            // Check for duplicate email if email is being updated
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != customer.Email)
            {
                var exists = await _context.Customers.AnyAsync(c => c.Email == dto.Email);
                if (exists)
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "A customer with this email already exists",
                        Errors = new Dictionary<string, string[]> { { "Email", new[] { "Email already in use" } } },
                        StatusCode = 400,
                    });
            }

            dto.UpdateEntity(customer);
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer updated: {CustomerId}", id);

            return Ok(customer.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while updating the customer",
                StatusCode = 500,
            });
        }
    }

    /// <summary>
    /// Delete a customer.
    /// </summary>
    /// <param name="id">Customer ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Customer with ID {id} not found",
                    StatusCode = 404,
                });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer deleted: {CustomerId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
            {
                Message = "An error occurred while deleting the customer",
                StatusCode = 500,
            });
        }
    }
}
