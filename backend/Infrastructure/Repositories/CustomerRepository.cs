using CateringQuotes.Application.Repositories;
using CateringQuotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CateringQuotes.Infrastructure.Repositories;

/// <summary>
/// Repository for Customer data access.
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly CateringQuotesDbContext _context;

    public CustomerRepository(CateringQuotesDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
    {
        var query = _context.Customers.Where(c => c.Email == email);

        if (excludeCustomerId.HasValue)
            query = query.Where(c => c.Id != excludeCustomerId.Value);

        return await query.AnyAsync();
    }
}
