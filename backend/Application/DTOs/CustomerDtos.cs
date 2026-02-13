namespace CateringQuotes.Application.DTOs;

/// <summary>
/// DTO for creating a new customer.
/// </summary>
public class CreateCustomerDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Company { get; set; }
}

/// <summary>
/// DTO for updating an existing customer.
/// </summary>
public class UpdateCustomerDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
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
