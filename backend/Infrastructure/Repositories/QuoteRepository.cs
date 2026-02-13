using CateringQuotes.Application.Repositories;
using CateringQuotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Infrastructure.Repositories;

/// <summary>
/// Repository for Quote data access.
/// </summary>
public class QuoteRepository : IQuoteRepository
{
    private readonly CateringQuotesDbContext _context;

    public QuoteRepository(CateringQuotesDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountQuotesByPrefixAsync(string prefix)
    {
        return await _context.Quotes
            .Where(q => q.QuoteNumber.StartsWith(prefix))
            .CountAsync();
    }

    public async Task<bool> QuoteExistsAsync(int quoteId)
    {
        return await _context.Quotes.AnyAsync(q => q.Id == quoteId);
    }
}
