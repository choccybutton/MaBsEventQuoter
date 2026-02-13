namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Application settings and defaults (should contain only one row).
/// </summary>
public class AppSettings
{
    public int Id { get; set; } = 1; // Single row only
    public decimal DefaultVatRate { get; set; } = 0.20m;
    public decimal DefaultMarkupPercentage { get; set; } = 0.70m;
    public decimal MarginGreenThresholdPct { get; set; } = 0.70m;
    public decimal MarginAmberThresholdPct { get; set; } = 0.60m;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}
