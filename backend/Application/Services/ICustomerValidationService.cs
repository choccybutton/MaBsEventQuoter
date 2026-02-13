using CateringQuotes.Application.DTOs;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for validating customer-related inputs.
/// </summary>
public interface ICustomerValidationService
{
    /// <summary>
    /// Validate create customer DTO.
    /// Throws ValidationException if validation fails.
    /// </summary>
    void ValidateCreateCustomerDto(CreateCustomerDto dto);

    /// <summary>
    /// Validate update customer DTO.
    /// Throws ValidationException if validation fails.
    /// </summary>
    void ValidateUpdateCustomerDto(UpdateCustomerDto dto);

    /// <summary>
    /// Validate email format and uniqueness.
    /// </summary>
    Task ValidateEmailUniqueAsync(string email, int? excludeCustomerId = null);
}
