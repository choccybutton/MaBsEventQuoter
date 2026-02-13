using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Exceptions;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for validating quote-related inputs.
/// </summary>
public class QuoteValidationService : IQuoteValidationService
{
    public void ValidateCreateQuoteDto(CreateQuoteDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validate customer ID
        if (dto.CustomerId <= 0)
            errors.Add("CustomerId", new[] { "Valid customer ID is required" });

        // Validate line items
        if (dto.LineItems == null || dto.LineItems.Count == 0)
            errors.Add("LineItems", new[] { "At least one line item is required" });
        else
            ValidateLineItems(dto.LineItems);

        // Validate VAT rate if provided
        if (dto.VatRate.HasValue && (dto.VatRate < 0 || dto.VatRate > 1))
            errors.Add("VatRate", new[] { "VAT rate must be between 0 and 1" });

        // Validate markup percentage if provided
        if (dto.MarkupPercentage.HasValue && dto.MarkupPercentage < 0)
            errors.Add("MarkupPercentage", new[] { "Markup percentage must be greater than or equal to 0" });

        if (errors.Count > 0)
            throw new ValidationException("Quote validation failed", errors);
    }

    public void ValidateUpdateQuoteDto(UpdateQuoteDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validate line items if provided
        if (dto.LineItems != null && dto.LineItems.Count > 0)
            ValidateLineItems(dto.LineItems);

        // Validate VAT rate if provided
        if (dto.VatRate.HasValue && (dto.VatRate < 0 || dto.VatRate > 1))
            errors.Add("VatRate", new[] { "VAT rate must be between 0 and 1" });

        // Validate markup percentage if provided
        if (dto.MarkupPercentage.HasValue && dto.MarkupPercentage < 0)
            errors.Add("MarkupPercentage", new[] { "Markup percentage must be greater than or equal to 0" });

        if (errors.Count > 0)
            throw new ValidationException("Quote validation failed", errors);
    }

    public void ValidateLineItems(List<CreateQuoteLineItemDto> lineItems)
    {
        var errors = new Dictionary<string, string[]>();

        for (int i = 0; i < lineItems.Count; i++)
        {
            var item = lineItems[i];
            var itemErrors = new List<string>();

            // Validate food item ID
            if (item.FoodItemId <= 0)
                itemErrors.Add($"Line {i + 1}: Valid food item ID is required");

            // Validate quantity
            if (item.Quantity <= 0)
                itemErrors.Add($"Line {i + 1}: Quantity must be greater than 0");

            // Validate unit cost
            if (item.UnitCost < 0)
                itemErrors.Add($"Line {i + 1}: Unit cost cannot be negative");

            if (itemErrors.Count > 0)
                errors.Add($"LineItem[{i}]", itemErrors.ToArray());
        }

        if (errors.Count > 0)
            throw new ValidationException("Line item validation failed", errors);
    }
}
