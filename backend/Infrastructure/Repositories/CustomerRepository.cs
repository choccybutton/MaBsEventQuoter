using CateringQuotes.Application.Repositories;
using CateringQuotes.Domain.Entities;
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

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _context.Customers.FindAsync(customerId);
    }

    public async Task<(List<Customer> Items, int Total)> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var query = _context.Customers;

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetWithQuotesAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.Quotes)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> DeleteAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
    {
        var query = _context.Customers.Where(c => c.Email == email);

        if (excludeCustomerId.HasValue)
            query = query.Where(c => c.Id != excludeCustomerId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> CustomerExistsAsync(int customerId)
    {
        return await _context.Customers.AnyAsync(c => c.Id == customerId);
    }
}
