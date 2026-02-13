using CateringQuotes.Application.Exceptions;
using CateringQuotes.Application.Repositories;
using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for managing quote operations and business logic.
/// </summary>
public class QuoteService : IQuoteService
{
    private readonly IQuoteRepository _quoteRepository;

    public QuoteService(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }

    /// <summary>
    /// Generate a unique quote number.
    /// Format: QT-YYYY-### where YYYY is year and ### is sequence counter.
    /// Example: QT-2026-001, QT-2026-002, etc.
    /// </summary>
    public async Task<string> GenerateQuoteNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"QT-{year}";

        // Count existing quotes for this year
        var count = await _quoteRepository.CountQuotesByPrefixAsync(prefix);

        // Return next sequence number
        return $"{prefix}-{count + 1:D3}";
    }

    /// <summary>
    /// Validate that a quote can be updated.
    /// Only Draft quotes can be updated (business rule).
    /// </summary>
    public void ValidateQuoteCanBeUpdated(Quote quote)
    {
        if (quote.Status != "Draft")
            throw new DomainException($"Only Draft quotes can be updated. This quote status is '{quote.Status}'.");
    }

    /// <summary>
    /// Validate that a quote can be deleted.
    /// Only Draft quotes can be deleted (business rule).
    /// </summary>
    public void ValidateQuoteCanBeDeleted(Quote quote)
    {
        if (quote.Status != "Draft")
            throw new DomainException($"Only Draft quotes can be deleted. This quote status is '{quote.Status}'.");
    }

    /// <summary>
    /// Check if a quote exists by ID.
    /// </summary>
    public async Task<bool> QuoteExistsAsync(int quoteId)
    {
        return await _quoteRepository.QuoteExistsAsync(quoteId);
    }
}
