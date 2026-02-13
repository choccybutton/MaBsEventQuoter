namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for calculating quote pricing and margins.
/// </summary>
public interface IQuotePricingService
{
    /// <summary>
    /// Quote pricing calculation result.
    /// </summary>
    public record QuotePricingResult(
        decimal TotalCost,
        decimal TotalPrice,
        decimal Margin,
        string MarginStatus);

    /// <summary>
    /// Calculate line item total with markup.
    /// </summary>
    decimal CalculateLineTotal(decimal unitCost, int quantity, decimal markupPercentage);

    /// <summary>
    /// Calculate complete quote pricing with all taxes and markup.
    /// </summary>
    QuotePricingResult CalculateQuotePricing(
        decimal totalCost,
        decimal markupPercentage,
        decimal vatRate,
        decimal greenThresholdPct,
        decimal amberThresholdPct);

    /// <summary>
    /// Determine margin health status based on thresholds.
    /// </summary>
    string DetermineMarginStatus(decimal margin, decimal greenThreshold, decimal amberThreshold);
}
