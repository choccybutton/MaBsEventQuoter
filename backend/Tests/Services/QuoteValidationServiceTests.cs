using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Exceptions;
using CateringQuotes.Application.Services;
using Xunit;

namespace CateringQuotes.Tests.Services;

/// <summary>
/// Unit tests for QuoteValidationService.
/// Tests validation of quote creation and update requests.
/// </summary>
public class QuoteValidationServiceTests
{
    private readonly IQuoteValidationService _service = new QuoteValidationService();

    #region ValidateCreateQuoteDto Tests

    [Fact]
    public void ValidateCreateQuoteDto_WithValidData_DoesNotThrow()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            CustomerId = 1,
            EventDate = DateTime.UtcNow.AddDays(7),
            VatRate = 0.20m,
            MarkupPercentage = 0.50m,
            Notes = "Test quote",
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new()
                {
                    FoodItemId = 1,
                    Description = "Item 1",
                    Quantity = 2,
                    UnitCost = 100m,
                    DisplayOrder = 1
                }
            }
        };

        // Act & Assert - should not throw
        _service.ValidateCreateQuoteDto(dto);
    }

    [Fact]
    public void ValidateCreateQuoteDto_WithInvalidCustomerId_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            CustomerId = 0, // Invalid
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = 100m }
            }
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateQuoteDto(dto));
        Assert.Contains("CustomerId", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateQuoteDto_WithNegativeCustomerId_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            CustomerId = -1, // Invalid
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = 100m }
            }
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateQuoteDto(dto));
        Assert.Contains("CustomerId", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateQuoteDto_WithNoLineItems_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            CustomerId = 1,
            LineItems = new List<CreateQuoteLineItemDto>() // Empty
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateQuoteDto(dto));
        Assert.Contains("LineItems", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateQuoteDto_WithInvalidVATRate_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            CustomerId = 1,
            VatRate = 1.5m, // Invalid (> 1)
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = 100m }
            }
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateQuoteDto(dto));
        Assert.Contains("VatRate", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateQuoteDto_WithNegativeMarkup_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateQuoteDto
        {
            CustomerId = 1,
            MarkupPercentage = -0.1m, // Invalid
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = 100m }
            }
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateQuoteDto(dto));
        Assert.Contains("MarkupPercentage", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateQuoteDto_WithOptionalFieldsNull_DoesNotThrow()
    {
        // Arrange - optional fields are null
        var dto = new CreateQuoteDto
        {
            CustomerId = 1,
            VatRate = null,
            MarkupPercentage = null,
            Notes = null,
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = 100m }
            }
        };

        // Act & Assert - should not throw
        _service.ValidateCreateQuoteDto(dto);
    }

    #endregion

    #region ValidateUpdateQuoteDto Tests

    [Fact]
    public void ValidateUpdateQuoteDto_WithValidData_DoesNotThrow()
    {
        // Arrange
        var dto = new UpdateQuoteDto
        {
            EventDate = DateTime.UtcNow.AddDays(7),
            VatRate = 0.20m,
            MarkupPercentage = 0.50m,
            Notes = "Updated notes",
            LineItems = new List<CreateQuoteLineItemDto>
            {
                new() { FoodItemId = 1, Description = "Item", Quantity = 2, UnitCost = 100m }
            }
        };

        // Act & Assert
        _service.ValidateUpdateQuoteDto(dto);
    }

    [Fact]
    public void ValidateUpdateQuoteDto_WithInvalidVATRate_ThrowsValidationException()
    {
        // Arrange
        var dto = new UpdateQuoteDto { VatRate = -0.1m }; // Invalid

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateUpdateQuoteDto(dto));
        Assert.Contains("VatRate", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateUpdateQuoteDto_WithAllNullFields_DoesNotThrow()
    {
        // Arrange - all fields null (valid for update)
        var dto = new UpdateQuoteDto
        {
            EventDate = null,
            VatRate = null,
            MarkupPercentage = null,
            Notes = null,
            LineItems = null
        };

        // Act & Assert
        _service.ValidateUpdateQuoteDto(dto);
    }

    #endregion

    #region ValidateLineItems Tests

    [Fact]
    public void ValidateLineItems_WithValidItems_DoesNotThrow()
    {
        // Arrange
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 1, Description = "Item 1", Quantity = 2, UnitCost = 100m, DisplayOrder = 1 },
            new() { FoodItemId = 2, Description = "Item 2", Quantity = 3, UnitCost = 50m, DisplayOrder = 2 }
        };

        // Act & Assert
        _service.ValidateLineItems(lineItems);
    }

    [Fact]
    public void ValidateLineItems_WithInvalidFoodItemId_ThrowsValidationException()
    {
        // Arrange
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 0, Description = "Item", Quantity = 1, UnitCost = 100m } // Invalid
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateLineItems(lineItems));
        Assert.NotEmpty(ex.Errors);
    }

    [Fact]
    public void ValidateLineItems_WithZeroQuantity_ThrowsValidationException()
    {
        // Arrange
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 1, Description = "Item", Quantity = 0, UnitCost = 100m } // Invalid
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateLineItems(lineItems));
        Assert.NotEmpty(ex.Errors);
    }

    [Fact]
    public void ValidateLineItems_WithNegativeQuantity_ThrowsValidationException()
    {
        // Arrange
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 1, Description = "Item", Quantity = -1, UnitCost = 100m } // Invalid
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateLineItems(lineItems));
        Assert.NotEmpty(ex.Errors);
    }

    [Fact]
    public void ValidateLineItems_WithNegativeUnitCost_ThrowsValidationException()
    {
        // Arrange
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = -100m } // Invalid
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateLineItems(lineItems));
        Assert.NotEmpty(ex.Errors);
    }

    [Fact]
    public void ValidateLineItems_WithMultipleInvalidItems_ReturnsAllErrors()
    {
        // Arrange
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 0, Description = "Item 1", Quantity = 1, UnitCost = 100m }, // Invalid food item
            new() { FoodItemId = 2, Description = "Item 2", Quantity = 0, UnitCost = 100m }  // Invalid quantity
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateLineItems(lineItems));
        Assert.NotEmpty(ex.Errors);
        Assert.True(ex.Errors.Count >= 2);
    }

    [Fact]
    public void ValidateLineItems_WithZeroCostItem_IsValid()
    {
        // Arrange - zero cost is allowed (e.g., promotional items)
        var lineItems = new List<CreateQuoteLineItemDto>
        {
            new() { FoodItemId = 1, Description = "Item", Quantity = 1, UnitCost = 0m }
        };

        // Act & Assert - should not throw
        _service.ValidateLineItems(lineItems);
    }

    #endregion
}
