namespace CateringQuotes.Application.DTOs;

/// <summary>
/// DTO for allergen information.
/// </summary>
public class AllergenDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for dietary tag information.
/// </summary>
public class DietaryTagDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for application settings.
/// </summary>
public class AppSettingsDto
{
    public int Id { get; set; }
    public decimal DefaultVatRate { get; set; }
    public decimal DefaultMarkupPercentage { get; set; }
    public decimal MarginGreenThresholdPct { get; set; }
    public decimal MarginAmberThresholdPct { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// DTO for updating application settings.
/// </summary>
public class UpdateAppSettingsDto
{
    public decimal? DefaultVatRate { get; set; }
    public decimal? DefaultMarkupPercentage { get; set; }
    public decimal? MarginGreenThresholdPct { get; set; }
    public decimal? MarginAmberThresholdPct { get; set; }
}

/// <summary>
/// DTO for API error response.
/// </summary>
public class ApiErrorResponse
{
    public string Message { get; set; } = null!;
    public Dictionary<string, string[]>? Errors { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
