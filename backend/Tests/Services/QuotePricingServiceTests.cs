using CateringQuotes.Application.Services;
using Xunit;

namespace CateringQuotes.Tests.Services;

/// <summary>
/// Unit tests for QuotePricingService pricing calculations.
/// Tests various scenarios for margin calculation, VAT application, and markup computation.
/// </summary>
public class QuotePricingServiceTests
{
    private readonly IQuotePricingService _service = new QuotePricingService();

    #region CalculateLineTotal Tests

    [Fact]
    public void CalculateLineTotal_WithValidInputs_ReturnsCorrectTotal()
    {
        // Arrange: unitCost=100, quantity=2, markup=0.50 (50%)
        var unitCost = 100m;
        var quantity = 2;
        var markupPercentage = 0.50m;

        // Act: lineTotal = 100 * 2 * (1 + 0.50) = 300
        var result = _service.CalculateLineTotal(unitCost, quantity, markupPercentage);

        // Assert
        Assert.Equal(300m, result);
    }

    [Fact]
    public void CalculateLineTotal_WithZeroMarkup_ReturnsBaseCost()
    {
        // Arrange
        var unitCost = 50m;
        var quantity = 4;
        var markupPercentage = 0m;

        // Act: lineTotal = 50 * 4 * (1 + 0) = 200
        var result = _service.CalculateLineTotal(unitCost, quantity, markupPercentage);

        // Assert
        Assert.Equal(200m, result);
    }

    [Fact]
    public void CalculateLineTotal_WithDecimalValues_CalculatesAccurately()
    {
        // Arrange
        var unitCost = 10.50m;
        var quantity = 3;
        var markupPercentage = 0.25m;

        // Act: lineTotal = 10.50 * 3 * 1.25 = 39.375
        var result = _service.CalculateLineTotal(unitCost, quantity, markupPercentage);

        // Assert
        Assert.Equal(39.375m, result);
    }

    [Fact]
    public void CalculateLineTotal_WithNegativeUnitCost_ThrowsArgumentException()
    {
        // Arrange
        var unitCost = -10m;
        var quantity = 2;
        var markupPercentage = 0.5m;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CalculateLineTotal(unitCost, quantity, markupPercentage));
        Assert.Contains("Unit cost cannot be negative", ex.Message);
    }

    [Fact]
    public void CalculateLineTotal_WithZeroQuantity_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CalculateLineTotal(100m, 0, 0.5m));
        Assert.Contains("Quantity must be greater than 0", ex.Message);
    }

    [Fact]
    public void CalculateLineTotal_WithNegativeMarkup_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CalculateLineTotal(100m, 2, -0.1m));
        Assert.Contains("Markup percentage cannot be negative", ex.Message);
    }

    #endregion

    #region CalculateQuotePricing Tests

    [Fact]
    public void CalculateQuotePricing_WithStandardRates_CalculatesCorrectly()
    {
        // Arrange: totalCost=1000, markup=0.50 (50%), VAT=0.20 (20%)
        var totalCost = 1000m;
        var markupPercentage = 0.50m;
        var vatRate = 0.20m;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var result = _service.CalculateQuotePricing(totalCost, markupPercentage, vatRate, greenThreshold, amberThreshold);

        // Assert
        // priceBeforeVat = 1000 * (1 + 0.50) = 1500
        // vat = 1500 * 0.20 = 300
        // totalPrice = 1500 + 300 = 1800
        // margin = (1800 - 1000) / 1800 = 0.444... ≈ 44.4%
        Assert.Equal(1000m, result.TotalCost);
        Assert.Equal(1800m, result.TotalPrice);
        Assert.True(result.Margin > 0.44m && result.Margin < 0.45m);
        Assert.Equal("green", result.MarginStatus);
    }

    [Fact]
    public void CalculateQuotePricing_WithNoVAT_CalculatesWithoutTax()
    {
        // Arrange
        var totalCost = 1000m;
        var markupPercentage = 0.50m;
        var vatRate = 0m;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var result = _service.CalculateQuotePricing(totalCost, markupPercentage, vatRate, greenThreshold, amberThreshold);

        // Assert
        // priceBeforeVat = 1000 * 1.50 = 1500
        // vat = 1500 * 0 = 0
        // totalPrice = 1500
        // margin = (1500 - 1000) / 1500 = 0.333... ≈ 33.3%
        Assert.Equal(1000m, result.TotalCost);
        Assert.Equal(1500m, result.TotalPrice);
        Assert.True(result.Margin > 0.33m && result.Margin < 0.34m);
    }

    [Fact]
    public void CalculateQuotePricing_WithZeroCost_ReturnsZeroMargin()
    {
        // Arrange
        var totalCost = 0m;
        var markupPercentage = 0.50m;
        var vatRate = 0.20m;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var result = _service.CalculateQuotePricing(totalCost, markupPercentage, vatRate, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal(0m, result.TotalCost);
        Assert.Equal(0m, result.TotalPrice);
        Assert.Equal(0m, result.Margin);
    }

    [Fact]
    public void CalculateQuotePricing_WithHighMarkup_CalculatesCorrectly()
    {
        // Arrange: 100% markup
        var totalCost = 1000m;
        var markupPercentage = 1.0m;
        var vatRate = 0.20m;
        var greenThreshold = 0.50m;
        var amberThreshold = 0.30m;

        // Act
        var result = _service.CalculateQuotePricing(totalCost, markupPercentage, vatRate, greenThreshold, amberThreshold);

        // Assert
        // priceBeforeVat = 1000 * 2 = 2000
        // vat = 2000 * 0.20 = 400
        // totalPrice = 2400
        // margin = (2400 - 1000) / 2400 = 0.583... ≈ 58.3%
        Assert.Equal(2400m, result.TotalPrice);
        Assert.True(result.Margin > 0.58m);
        Assert.Equal("green", result.MarginStatus);
    }

    [Fact]
    public void CalculateQuotePricing_WithNegativeCost_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CalculateQuotePricing(-100m, 0.5m, 0.2m, 0.4m, 0.2m));
        Assert.Contains("Total cost cannot be negative", ex.Message);
    }

    [Fact]
    public void CalculateQuotePricing_WithNegativeMarkup_ThrowsArgumentException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CalculateQuotePricing(1000m, -0.1m, 0.2m, 0.4m, 0.2m));
        Assert.Contains("Markup percentage cannot be negative", ex.Message);
    }

    [Fact]
    public void CalculateQuotePricing_WithInvalidVATRate_ThrowsArgumentException()
    {
        // Act & Assert - VAT rate > 1
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.CalculateQuotePricing(1000m, 0.5m, 1.5m, 0.4m, 0.2m));
        Assert.Contains("VAT rate must be between 0 and 1", ex.Message);
    }

    #endregion

    #region DetermineMarginStatus Tests

    [Fact]
    public void DetermineMarginStatus_WithHighMargin_ReturnsGreen()
    {
        // Arrange
        var margin = 0.45m; // 45%
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var status = _service.DetermineMarginStatus(margin, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal("green", status);
    }

    [Fact]
    public void DetermineMarginStatus_WithMiddleMargin_ReturnsAmber()
    {
        // Arrange
        var margin = 0.30m; // 30%
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var status = _service.DetermineMarginStatus(margin, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal("amber", status);
    }

    [Fact]
    public void DetermineMarginStatus_WithLowMargin_ReturnsRed()
    {
        // Arrange
        var margin = 0.10m; // 10%
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var status = _service.DetermineMarginStatus(margin, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal("red", status);
    }

    [Fact]
    public void DetermineMarginStatus_AtGreenThreshold_ReturnsGreen()
    {
        // Arrange: exactly at green threshold
        var margin = 0.40m;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var status = _service.DetermineMarginStatus(margin, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal("green", status);
    }

    [Fact]
    public void DetermineMarginStatus_AtAmberThreshold_ReturnsAmber()
    {
        // Arrange: exactly at amber threshold
        var margin = 0.20m;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var status = _service.DetermineMarginStatus(margin, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal("amber", status);
    }

    [Fact]
    public void DetermineMarginStatus_BelowAmberThreshold_ReturnsRed()
    {
        // Arrange: below amber threshold
        var margin = 0.19m;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var status = _service.DetermineMarginStatus(margin, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal("red", status);
    }

    #endregion

    #region Integration/Realistic Scenario Tests

    [Theory]
    // Calculations: lineTotal = unitCost * quantity * (1 + markup)
    // totalCost (for pricing) = unitCost * quantity
    // priceBeforeVat = totalCost * (1 + markup)
    // vat = priceBeforeVat * vatRate
    // totalPrice = priceBeforeVat + vat
    [InlineData(100, 10, 0.50, 0.20, 1800)]   // 1000*1.5*1.2 = 1800
    [InlineData(50, 20, 0.70, 0.20, 2040)]    // 1000*1.7*1.2 = 2040
    [InlineData(1000, 1, 0.30, 0.20, 1560)]   // 1000*1.3*1.2 = 1560
    public void CalculateLineTotal_ThenQuotePricing_ProducesCorrectResults(
        decimal unitCost, int quantity, decimal markup, decimal vat, decimal expectedPrice)
    {
        // Arrange
        var totalCost = unitCost * quantity;
        var greenThreshold = 0.40m;
        var amberThreshold = 0.20m;

        // Act
        var result = _service.CalculateQuotePricing(totalCost, markup, vat, greenThreshold, amberThreshold);

        // Assert
        Assert.Equal(expectedPrice, result.TotalPrice);
    }

    #endregion
}
