namespace CateringQuotes.Domain.Entities;

/// <summary>
/// Lookup table for dietary information (vegan, gluten-free, etc).
/// </summary>
public class DietaryTag
{
    public int Id { get; set; }
    public string Code { get; set; } = null!; // e.g., "VEGAN", "GLUTEN_FREE"
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public override string ToString() => $"{Code} - {Name}";
}
