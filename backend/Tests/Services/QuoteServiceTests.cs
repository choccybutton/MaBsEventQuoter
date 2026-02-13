using CateringQuotes.Application.Exceptions;
using CateringQuotes.Application.Repositories;
using CateringQuotes.Application.Services;
using CateringQuotes.Domain.Entities;
using Moq;
using Xunit;

namespace CateringQuotes.Tests.Services;

/// <summary>
/// Unit tests for QuoteService.
/// Tests quote number generation, state validation, and existence checks.
/// </summary>
public class QuoteServiceTests
{
    private readonly Mock<IQuoteRepository> _mockRepository = new();
    private readonly IQuoteService _service;

    public QuoteServiceTests()
    {
        _service = new QuoteService(_mockRepository.Object);
    }

    #region GenerateQuoteNumberAsync Tests

    [Fact]
    public async Task GenerateQuoteNumberAsync_WithNoExistingQuotes_ReturnsFirstNumber()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        _mockRepository
            .Setup(r => r.CountQuotesByPrefixAsync($"QT-{currentYear}"))
            .ReturnsAsync(0);

        // Act
        var result = await _service.GenerateQuoteNumberAsync();

        // Assert
        Assert.Equal($"QT-{currentYear}-001", result);
    }

    [Fact]
    public async Task GenerateQuoteNumberAsync_WithExistingQuotes_ReturnsIncrementedNumber()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        _mockRepository
            .Setup(r => r.CountQuotesByPrefixAsync($"QT-{currentYear}"))
            .ReturnsAsync(5);

        // Act
        var result = await _service.GenerateQuoteNumberAsync();

        // Assert
        Assert.Equal($"QT-{currentYear}-006", result);
    }

    [Fact]
    public async Task GenerateQuoteNumberAsync_WithManyQuotes_PadsWithZeros()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        _mockRepository
            .Setup(r => r.CountQuotesByPrefixAsync($"QT-{currentYear}"))
            .ReturnsAsync(100);

        // Act
        var result = await _service.GenerateQuoteNumberAsync();

        // Assert
        Assert.Equal($"QT-{currentYear}-101", result);
        Assert.Matches(@"QT-\d{4}-\d{3}", result);
    }

    [Fact]
    public async Task GenerateQuoteNumberAsync_CallsRepositoryWithCorrectPrefix()
    {
        // Arrange
        var currentYear = DateTime.UtcNow.Year;
        _mockRepository
            .Setup(r => r.CountQuotesByPrefixAsync($"QT-{currentYear}"))
            .ReturnsAsync(0);

        // Act
        await _service.GenerateQuoteNumberAsync();

        // Assert
        _mockRepository.Verify(
            r => r.CountQuotesByPrefixAsync($"QT-{currentYear}"),
            Times.Once);
    }

    #endregion

    #region ValidateQuoteCanBeUpdated Tests

    [Fact]
    public void ValidateQuoteCanBeUpdated_WithDraftQuote_DoesNotThrow()
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = "Draft" };

        // Act & Assert - should not throw
        _service.ValidateQuoteCanBeUpdated(quote);
    }

    [Fact]
    public void ValidateQuoteCanBeUpdated_WithSentQuote_ThrowsDomainException()
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = "Sent" };

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => _service.ValidateQuoteCanBeUpdated(quote));
        Assert.Contains("Only Draft quotes can be updated", ex.Message);
        Assert.Contains("Sent", ex.Message);
    }

    [Fact]
    public void ValidateQuoteCanBeUpdated_WithAcceptedQuote_ThrowsDomainException()
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = "Accepted" };

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => _service.ValidateQuoteCanBeUpdated(quote));
        Assert.Contains("Only Draft quotes can be updated", ex.Message);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Sent")]
    [InlineData("Accepted")]
    [InlineData("Rejected")]
    [InlineData("Archived")]
    public void ValidateQuoteCanBeUpdated_OnlyAllowsDraftStatus(string status)
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = status };

        // Act & Assert
        if (status == "Draft")
        {
            _service.ValidateQuoteCanBeUpdated(quote); // Should not throw
        }
        else
        {
            Assert.Throws<DomainException>(() => _service.ValidateQuoteCanBeUpdated(quote));
        }
    }

    #endregion

    #region ValidateQuoteCanBeDeleted Tests

    [Fact]
    public void ValidateQuoteCanBeDeleted_WithDraftQuote_DoesNotThrow()
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = "Draft" };

        // Act & Assert - should not throw
        _service.ValidateQuoteCanBeDeleted(quote);
    }

    [Fact]
    public void ValidateQuoteCanBeDeleted_WithSentQuote_ThrowsDomainException()
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = "Sent" };

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => _service.ValidateQuoteCanBeDeleted(quote));
        Assert.Contains("Only Draft quotes can be deleted", ex.Message);
    }

    [Fact]
    public void ValidateQuoteCanBeDeleted_WithAcceptedQuote_ThrowsDomainException()
    {
        // Arrange
        var quote = new Quote { Id = 1, Status = "Accepted" };

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => _service.ValidateQuoteCanBeDeleted(quote));
        Assert.Contains("Only Draft quotes can be deleted", ex.Message);
    }

    #endregion

    #region QuoteExistsAsync Tests

    [Fact]
    public async Task QuoteExistsAsync_WithExistingQuote_ReturnsTrue()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.QuoteExistsAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.QuoteExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task QuoteExistsAsync_WithNonExistentQuote_ReturnsFalse()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.QuoteExistsAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _service.QuoteExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task QuoteExistsAsync_CallsRepositoryWithCorrectId()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.QuoteExistsAsync(5))
            .ReturnsAsync(true);

        // Act
        await _service.QuoteExistsAsync(5);

        // Assert
        _mockRepository.Verify(r => r.QuoteExistsAsync(5), Times.Once);
    }

    #endregion
}
