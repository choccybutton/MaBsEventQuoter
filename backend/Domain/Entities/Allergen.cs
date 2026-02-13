namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Lookup table for allergen information.
/// </summary>
public class Allergen
{
    public int Id { get; set; }
    public string Code { get; set; } = null!; // e.g., "CELERY", "PEANUTS"
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public override string ToString() => $"{Code} - {Name}";
}
