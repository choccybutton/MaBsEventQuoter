using System.ComponentModel.DataAnnotations;

namespace CateringQuotes.Application.DTOs;

/// <summary>
/// DTO for a quote line item.
/// </summary>
public class QuoteLineItemDto
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public int FoodItemId { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// DTO for creating a new quote.
/// </summary>
public class CreateQuoteDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Customer ID must be greater than 0")]
    public int CustomerId { get; set; }

    public DateTime? EventDate { get; set; }

    [Range(0, 1, ErrorMessage = "VAT rate must be between 0 and 1")]
    public decimal? VatRate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Markup percentage must be greater than or equal to 0")]
    public decimal? MarkupPercentage { get; set; }

    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? Notes { get; set; }

    public List<CreateQuoteLineItemDto> LineItems { get; set; } = new();
}

/// <summary>
/// DTO for creating a quote line item.
/// </summary>
public class CreateQuoteLineItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Food item ID must be greater than 0")]
    public int FoodItemId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(255, ErrorMessage = "Description must not exceed 255 characters")]
    public string Description { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Unit cost must be greater than or equal to 0")]
    public decimal UnitCost { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Display order must be greater than or equal to 0")]
    public int DisplayOrder { get; set; }
}

/// <summary>
/// DTO for updating a quote.
/// </summary>
public class UpdateQuoteDto
{
    public DateTime? EventDate { get; set; }

    [Range(0, 1, ErrorMessage = "VAT rate must be between 0 and 1")]
    public decimal? VatRate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Markup percentage must be greater than or equal to 0")]
    public decimal? MarkupPercentage { get; set; }

    [StringLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
    public string? Notes { get; set; }

    public List<CreateQuoteLineItemDto>? LineItems { get; set; }
}

/// <summary>
/// DTO for displaying quote information.
/// </summary>
public class QuoteDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public CustomerDto? Customer { get; set; }
    public string QuoteNumber { get; set; } = null!;
    public DateTime QuoteDate { get; set; }
    public DateTime? EventDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal VatRate { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Margin { get; set; }
    public decimal MarkupPercentage { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public List<QuoteLineItemDto> LineItems { get; set; } = new();
}

/// <summary>
/// DTO for sending a quote via email.
/// </summary>
public class SendQuoteDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    public string Email { get; set; } = null!;
}

/// <summary>
/// DTO for paginated response.
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (Total + PageSize - 1) / PageSize;
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
