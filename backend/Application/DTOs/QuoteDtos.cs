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
    public int CustomerId { get; set; }
    public DateTime? EventDate { get; set; }
    public decimal? VatRate { get; set; }
    public decimal? MarkupPercentage { get; set; }
    public string? Notes { get; set; }
    public List<CreateQuoteLineItemDto> LineItems { get; set; } = new();
}

/// <summary>
/// DTO for creating a quote line item.
/// </summary>
public class CreateQuoteLineItemDto
{
    public int FoodItemId { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// DTO for updating a quote.
/// </summary>
public class UpdateQuoteDto
{
    public DateTime? EventDate { get; set; }
    public decimal? VatRate { get; set; }
    public decimal? MarkupPercentage { get; set; }
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
