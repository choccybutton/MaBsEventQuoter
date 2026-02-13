using CateringQuotes.Domain.Entities;

namespace CateringQuotes.Application.Repositories;

/// <summary>
/// Repository interface for Customer data access.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Check if a customer with the given email exists (excluding specific customer ID if provided).
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null);
}
