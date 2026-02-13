using CateringQuotes.Application.Repositories;
using CateringQuotes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace CateringQuotes.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation for coordinating repositories and transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CateringQuotesDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy-loaded repositories
    private IQuoteRepository? _quoteRepository;
    private ICustomerRepository? _customerRepository;
    private IFoodItemRepository? _foodItemRepository;

    public UnitOfWork(CateringQuotesDbContext context)
    {
        _context = context;
    }

    public IQuoteRepository Quotes =>
        _quoteRepository ??= new QuoteRepository(_context);

    public ICustomerRepository Customers =>
        _customerRepository ??= new CustomerRepository(_context);

    public IFoodItemRepository FoodItems =>
        _foodItemRepository ??= new FoodItemRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            await _transaction?.RollbackAsync()!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _transaction?.Dispose();
        await _context.DisposeAsync();
    }
}
