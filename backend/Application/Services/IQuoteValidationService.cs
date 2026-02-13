using CateringQuotes.Application.DTOs;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for validating quote-related inputs.
/// </summary>
public interface IQuoteValidationService
{
    /// <summary>
    /// Validate create quote DTO.
    /// Throws ValidationException if validation fails.
    /// </summary>
    void ValidateCreateQuoteDto(CreateQuoteDto dto);

    /// <summary>
    /// Validate update quote DTO.
    /// Throws ValidationException if validation fails.
    /// </summary>
    void ValidateUpdateQuoteDto(UpdateQuoteDto dto);

    /// <summary>
    /// Validate line items in a quote.
    /// Throws ValidationException if invalid.
    /// </summary>
    void ValidateLineItems(List<CreateQuoteLineItemDto> lineItems);
}
