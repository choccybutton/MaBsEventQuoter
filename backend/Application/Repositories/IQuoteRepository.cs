using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Repositories;

/// <summary>
/// Repository interface for Quote data access.
/// </summary>
public interface IQuoteRepository
{
    /// <summary>
    /// Get quote by ID with related entities.
    /// </summary>
    Task<Quote?> GetByIdAsync(int quoteId);

    /// <summary>
    /// Get all quotes with pagination.
    /// </summary>
    Task<(List<Quote> Items, int Total)> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Get quotes filtered by status and/or customer ID.
    /// </summary>
    Task<(List<Quote> Items, int Total)> GetFilteredAsync(
        int page = 1,
        int pageSize = 20,
        string? status = null,
        int? customerId = null);

    /// <summary>
    /// Get quote by quote number.
    /// </summary>
    Task<Quote?> GetByQuoteNumberAsync(string quoteNumber);

    /// <summary>
    /// Get all quotes for a specific customer.
    /// </summary>
    Task<List<Quote>> GetByCustomerIdAsync(int customerId);

    /// <summary>
    /// Create a new quote.
    /// </summary>
    Task<Quote> CreateAsync(Quote quote);

    /// <summary>
    /// Update an existing quote.
    /// </summary>
    Task<Quote> UpdateAsync(Quote quote);

    /// <summary>
    /// Delete a quote by ID.
    /// </summary>
    Task<bool> DeleteAsync(int quoteId);

    /// <summary>
    /// Count quotes matching the given prefix.
    /// </summary>
    Task<int> CountQuotesByPrefixAsync(string prefix);

    /// <summary>
    /// Check if a quote exists by ID.
    /// </summary>
    Task<bool> QuoteExistsAsync(int quoteId);
}
