using System.ComponentModel.DataAnnotations;

namespace CateringQuotes.Application.DTOs;

/// <summary>
/// DTO for creating a new customer.
/// </summary>
public class CreateCustomerDto
{
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 255 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(255, ErrorMessage = "Email must not exceed 255 characters")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "Phone number format is invalid")]
    [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters")]
    public string? Phone { get; set; }

    [StringLength(255, ErrorMessage = "Company name must not exceed 255 characters")]
    public string? Company { get; set; }
}

/// <summary>
/// DTO for updating an existing customer.
/// </summary>
public class UpdateCustomerDto
{
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 255 characters")]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(255, ErrorMessage = "Email must not exceed 255 characters")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Phone number format is invalid")]
    [StringLength(20, ErrorMessage = "Phone must not exceed 20 characters")]
    public string? Phone { get; set; }

    [StringLength(255, ErrorMessage = "Company name must not exceed 255 characters")]
    public string? Company { get; set; }
}

/// <summary>
/// DTO for displaying customer information.
/// </summary>
public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
