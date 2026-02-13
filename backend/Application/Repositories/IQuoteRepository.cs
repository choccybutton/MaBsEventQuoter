using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Repositories;

/// <summary>
/// Repository interface for Quote data access.
/// </summary>
public interface IQuoteRepository
{
    /// <summary>
    /// Count quotes matching the given prefix.
    /// </summary>
    Task<int> CountQuotesByPrefixAsync(string prefix);

    /// <summary>
    /// Check if a quote exists by ID.
    /// </summary>
    Task<bool> QuoteExistsAsync(int quoteId);
}
