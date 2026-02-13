namespace CateringQuotes.Application.Repositories;

/// <summary>
/// Unit of Work pattern for coordinating multiple repositories and transactions.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Quote repository.
    /// </summary>
    IQuoteRepository Quotes { get; }

    /// <summary>
    /// Customer repository.
    /// </summary>
    ICustomerRepository Customers { get; }

    /// <summary>
    /// Food item repository.
    /// </summary>
    IFoodItemRepository FoodItems { get; }

    /// <summary>
    /// Save all changes as a transaction.
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begin a database transaction.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit the current transaction.
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rollback the current transaction.
    /// </summary>
    Task RollbackAsync();
}
