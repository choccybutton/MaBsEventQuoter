namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Represents a customer who can request catering quotes.
/// </summary>
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    // Navigation
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
