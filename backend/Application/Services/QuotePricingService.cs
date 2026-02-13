namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for calculating quote pricing and margins.
/// </summary>
public class QuotePricingService : IQuotePricingService
{
    /// <summary>
    /// Calculate line item total with markup applied.
    /// Formula: unitCost * quantity * (1 + markupPercentage)
    /// </summary>
    public decimal CalculateLineTotal(decimal unitCost, int quantity, decimal markupPercentage)
    {
        if (unitCost < 0) throw new ArgumentException("Unit cost cannot be negative");
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than 0");
        if (markupPercentage < 0) throw new ArgumentException("Markup percentage cannot be negative");

        var lineCost = unitCost * quantity;
        return lineCost * (1 + markupPercentage);
    }

    /// <summary>
    /// Calculate complete quote pricing with all taxes and markup.
    ///
    /// Pricing Formula:
    /// 1. Line totals = sum of (unitCost * quantity * (1 + markup))
    /// 2. Price before VAT = Line totals (markup already applied)
    /// 3. VAT = Price before VAT * vatRate
    /// 4. Total Price = Price before VAT + VAT
    /// 5. Margin = (Total Price - Total Cost) / Total Price
    ///
    /// Note: Total Cost is the sum of all unitCost * quantity (without markup/VAT)
    /// </summary>
    public IQuotePricingService.QuotePricingResult CalculateQuotePricing(
        decimal totalCost,
        decimal markupPercentage,
        decimal vatRate,
        decimal greenThresholdPct,
        decimal amberThresholdPct)
    {
        if (totalCost < 0) throw new ArgumentException("Total cost cannot be negative");
        if (markupPercentage < 0) throw new ArgumentException("Markup percentage cannot be negative");
        if (vatRate < 0 || vatRate > 1) throw new ArgumentException("VAT rate must be between 0 and 1");

        // Apply markup to cost
        var priceBeforeVat = totalCost * (1 + markupPercentage);

        // Apply VAT
        var vat = priceBeforeVat * vatRate;
        var totalPrice = priceBeforeVat + vat;

        // Calculate margin: (Total Price - Total Cost) / Total Price
        // Edge case: if totalPrice is 0, margin is 0
        var margin = totalPrice > 0 ? (totalPrice - totalCost) / totalPrice : 0;

        // Determine margin status
        var marginStatus = DetermineMarginStatus(margin, greenThresholdPct, amberThresholdPct);

        return new IQuotePricingService.QuotePricingResult(
            TotalCost: totalCost,
            TotalPrice: totalPrice,
            Margin: margin,
            MarginStatus: marginStatus);
    }

    /// <summary>
    /// Determine margin health status based on thresholds.
    /// Green: margin >= greenThreshold
    /// Amber: margin >= amberThreshold && margin < greenThreshold
    /// Red: margin < amberThreshold
    /// </summary>
    public string DetermineMarginStatus(decimal margin, decimal greenThreshold, decimal amberThreshold)
    {
        if (margin >= greenThreshold)
            return "green";
        else if (margin >= amberThreshold)
            return "amber";
        else
            return "red";
    }
}
