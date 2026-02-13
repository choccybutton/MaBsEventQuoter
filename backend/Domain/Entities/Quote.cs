namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Represents a catering quote with items and pricing.
/// </summary>
public class Quote
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string QuoteNumber { get; set; } = null!;
    public DateTime QuoteDate { get; set; } = DateTime.UtcNow;
    public DateTime? EventDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Accepted, Rejected, Completed
    public decimal VatRate { get; set; } = 0.20m; // 20%
    public decimal TotalCost { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal Margin { get; set; }
    public decimal MarkupPercentage { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public DateTime? SentAt { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public ICollection<QuoteLineItem> LineItems { get; set; } = new List<QuoteLineItem>();
}
