using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Exceptions;
using CateringQuotes.Application.Repositories;

namespace CateringQuotes.Application.Services;

/// <summary>
/// Service for validating customer-related inputs.
/// </summary>
public class CustomerValidationService : ICustomerValidationService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerValidationService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public void ValidateCreateCustomerDto(CreateCustomerDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validate name
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Name", new[] { "Customer name is required" });

        // Validate email
        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add("Email", new[] { "Email is required" });
        else if (!IsValidEmail(dto.Email))
            errors.Add("Email", new[] { "Email format is invalid" });

        if (errors.Count > 0)
            throw new ValidationException("Customer validation failed", errors);
    }

    public void ValidateUpdateCustomerDto(UpdateCustomerDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        // Validate name if provided
        if (!string.IsNullOrWhiteSpace(dto.Name) && string.IsNullOrWhiteSpace(dto.Name.Trim()))
            errors.Add("Name", new[] { "Customer name cannot be empty" });

        // Validate email if provided
        if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
            errors.Add("Email", new[] { "Email format is invalid" });

        if (errors.Count > 0)
            throw new ValidationException("Customer validation failed", errors);
    }

    public async Task ValidateEmailUniqueAsync(string email, int? excludeCustomerId = null)
    {
        var exists = await _customerRepository.EmailExistsAsync(email, excludeCustomerId);

        if (exists)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "A customer with this email already exists" } }
            };
            throw new ValidationException("Email is not unique", errors);
        }
    }

    /// <summary>
    /// Simple email validation using MailAddress.
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
