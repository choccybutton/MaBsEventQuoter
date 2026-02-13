using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Repositories;

/// <summary>
/// Repository interface for Customer data access.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Get customer by ID.
    /// </summary>
    Task<Customer?> GetByIdAsync(int customerId);

    /// <summary>
    /// Get all customers with pagination.
    /// </summary>
    Task<(List<Customer> Items, int Total)> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Get customer by email.
    /// </summary>
    Task<Customer?> GetByEmailAsync(string email);

    /// <summary>
    /// Get customer with all related quotes.
    /// </summary>
    Task<Customer?> GetWithQuotesAsync(int customerId);

    /// <summary>
    /// Create a new customer.
    /// </summary>
    Task<Customer> CreateAsync(Customer customer);

    /// <summary>
    /// Update an existing customer.
    /// </summary>
    Task<Customer> UpdateAsync(Customer customer);

    /// <summary>
    /// Delete a customer by ID.
    /// </summary>
    Task<bool> DeleteAsync(int customerId);

    /// <summary>
    /// Check if a customer with the given email exists (excluding specific customer ID if provided).
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null);

    /// <summary>
    /// Check if a customer exists by ID.
    /// </summary>
    Task<bool> CustomerExistsAsync(int customerId);
}
