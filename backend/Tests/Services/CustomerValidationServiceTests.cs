using CateringQuotes.Application.DTOs;
using CateringQuotes.Application.Exceptions;
using CateringQuotes.Application.Repositories;
using CateringQuotes.Application.Services;
using Moq;
using Xunit;

namespace CateringQuotes.Tests.Services;

/// <summary>
/// Unit tests for CustomerValidationService.
/// Tests validation of customer creation and updates, including email uniqueness.
/// </summary>
public class CustomerValidationServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository = new();
    private readonly ICustomerValidationService _service;

    public CustomerValidationServiceTests()
    {
        _service = new CustomerValidationService(_mockRepository.Object);
    }

    #region ValidateCreateCustomerDto Tests

    [Fact]
    public void ValidateCreateCustomerDto_WithValidData_DoesNotThrow()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Company = "Acme Corp"
        };

        // Act & Assert - should not throw
        _service.ValidateCreateCustomerDto(dto);
    }

    [Fact]
    public void ValidateCreateCustomerDto_WithMissingName_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            Name = "", // Empty
            Email = "john@example.com",
            Phone = "1234567890"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateCustomerDto(dto));
        Assert.Contains("Name", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateCustomerDto_WithMissingEmail_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            Name = "John Doe",
            Email = "", // Empty
            Phone = "1234567890"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateCustomerDto(dto));
        Assert.Contains("Email", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateCustomerDto_WithInvalidEmail_ThrowsValidationException()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            Name = "John Doe",
            Email = "invalid-email", // Invalid format
            Phone = "1234567890"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateCustomerDto(dto));
        Assert.Contains("Email", ex.Errors.Keys);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("firstname+lastname@example.com")]
    public void ValidateCreateCustomerDto_WithValidEmails_DoesNotThrow(string email)
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            Name = "John Doe",
            Email = email,
            Phone = null
        };

        // Act & Assert
        _service.ValidateCreateCustomerDto(dto);
    }

    [Theory]
    [InlineData("")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user @example.com")]
    public void ValidateCreateCustomerDto_WithInvalidEmailFormats_ThrowsValidationException(string email)
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            Name = "John Doe",
            Email = email
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateCreateCustomerDto(dto));
        Assert.Contains("Email", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateCreateCustomerDto_WithOptionalFields_DoesNotThrow()
    {
        // Arrange - optional fields null
        var dto = new CreateCustomerDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = null,
            Company = null
        };

        // Act & Assert
        _service.ValidateCreateCustomerDto(dto);
    }

    #endregion

    #region ValidateUpdateCustomerDto Tests

    [Fact]
    public void ValidateUpdateCustomerDto_WithValidData_DoesNotThrow()
    {
        // Arrange
        var dto = new UpdateCustomerDto
        {
            Name = "Jane Doe",
            Email = "jane@example.com",
            Phone = "0987654321"
        };

        // Act & Assert
        _service.ValidateUpdateCustomerDto(dto);
    }

    [Fact]
    public void ValidateUpdateCustomerDto_WithInvalidEmail_ThrowsValidationException()
    {
        // Arrange
        var dto = new UpdateCustomerDto { Email = "invalid-email" };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => _service.ValidateUpdateCustomerDto(dto));
        Assert.Contains("Email", ex.Errors.Keys);
    }

    [Fact]
    public void ValidateUpdateCustomerDto_WithAllNullFields_DoesNotThrow()
    {
        // Arrange - all fields null (valid for update)
        var dto = new UpdateCustomerDto
        {
            Name = null,
            Email = null,
            Phone = null,
            Company = null
        };

        // Act & Assert
        _service.ValidateUpdateCustomerDto(dto);
    }

    [Fact]
    public void ValidateUpdateCustomerDto_WithValidName_DoesNotThrow()
    {
        // Arrange
        var dto = new UpdateCustomerDto { Name = "Valid Name" };

        // Act & Assert
        _service.ValidateUpdateCustomerDto(dto);
    }

    #endregion

    #region ValidateEmailUniqueAsync Tests

    [Fact]
    public async Task ValidateEmailUniqueAsync_WithUniqueEmail_DoesNotThrow()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.EmailExistsAsync("unique@example.com", null))
            .ReturnsAsync(false);

        // Act & Assert - should not throw
        await _service.ValidateEmailUniqueAsync("unique@example.com");
    }

    [Fact]
    public async Task ValidateEmailUniqueAsync_WithDuplicateEmail_ThrowsValidationException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.EmailExistsAsync("duplicate@example.com", null))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => _service.ValidateEmailUniqueAsync("duplicate@example.com"));
        Assert.Contains("Email", ex.Errors.Keys);
    }

    [Fact]
    public async Task ValidateEmailUniqueAsync_WhenExcludingCustomerId_IgnoresThatCustomer()
    {
        // Arrange - email exists but belongs to customer 1 (which we're excluding)
        _mockRepository
            .Setup(r => r.EmailExistsAsync("email@example.com", 1))
            .ReturnsAsync(false);

        // Act & Assert - should not throw
        await _service.ValidateEmailUniqueAsync("email@example.com", 1);
    }

    [Fact]
    public async Task ValidateEmailUniqueAsync_WithExcludedCustomerButDifferentOwner_ThrowsValidationException()
    {
        // Arrange - email exists but belongs to customer 2, not customer 1
        _mockRepository
            .Setup(r => r.EmailExistsAsync("email@example.com", 1))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => _service.ValidateEmailUniqueAsync("email@example.com", 1));
        Assert.Contains("Email", ex.Errors.Keys);
    }

    #endregion
}
