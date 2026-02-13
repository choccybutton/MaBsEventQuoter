using CateringQuotes.Application.Repositories;
using CateringQuotes.Domain.Entities;
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

    public async Task<Quote?> GetByIdAsync(int quoteId)
    {
        return await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.Id == quoteId);
    }

    public async Task<(List<Quote> Items, int Total)> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.LineItems);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(q => q.QuoteDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(List<Quote> Items, int Total)> GetFilteredAsync(
        int page = 1,
        int pageSize = 20,
        string? status = null,
        int? customerId = null)
    {
        var query = _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.LineItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(q => q.Status == status);

        if (customerId.HasValue)
            query = query.Where(q => q.CustomerId == customerId);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(q => q.QuoteDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Quote?> GetByQuoteNumberAsync(string quoteNumber)
    {
        return await _context.Quotes
            .Include(q => q.Customer)
            .Include(q => q.LineItems)
            .FirstOrDefaultAsync(q => q.QuoteNumber == quoteNumber);
    }

    public async Task<List<Quote>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Quotes
            .Where(q => q.CustomerId == customerId)
            .Include(q => q.LineItems)
            .OrderByDescending(q => q.QuoteDate)
            .ToListAsync();
    }

    public async Task<Quote> CreateAsync(Quote quote)
    {
        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<Quote> UpdateAsync(Quote quote)
    {
        _context.Quotes.Update(quote);
        await _context.SaveChangesAsync();
        return quote;
    }

    public async Task<bool> DeleteAsync(int quoteId)
    {
        var quote = await _context.Quotes.FindAsync(quoteId);
        if (quote == null)
            return false;

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();
        return true;
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
