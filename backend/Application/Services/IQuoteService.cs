using CateringQuotes.Application.Repositories;
using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for managing quote operations and business logic.
/// </summary>
public interface IQuoteService
{
    /// <summary>
    /// Generate a unique quote number.
    /// Format: QT-YYYY-### where YYYY is year and ### is sequence.
    /// </summary>
    Task<string> GenerateQuoteNumberAsync();

    /// <summary>
    /// Validate that a quote can be updated.
    /// Only Draft quotes can be updated.
    /// </summary>
    void ValidateQuoteCanBeUpdated(Quote quote);

    /// <summary>
    /// Validate that a quote can be deleted.
    /// Only Draft quotes can be deleted.
    /// </summary>
    void ValidateQuoteCanBeDeleted(Quote quote);

    /// <summary>
    /// Check if a quote exists by ID.
    /// </summary>
    Task<bool> QuoteExistsAsync(int quoteId);
}
