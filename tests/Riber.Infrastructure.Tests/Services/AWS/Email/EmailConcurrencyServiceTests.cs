using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.AWS.Email;

namespace Riber.Infrastructure.Tests.Services.AWS.Email;

public sealed class EmailConcurrencyServiceTests : BaseTest
{
    #region Constructor Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should use default max concurrent emails when configuration is missing")]
    public void Constructor_WhenConfigurationIsMissing_ShouldUseDefaultValue()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();

        // Act
        var act = () => new EmailConcurrencyService(configuration);

        // Assert
        act.Should().NotThrow();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should use configured max concurrent emails")]
    public void Constructor_WhenConfigurationHasValue_ShouldUseConfiguredValue()
    {
        // Arrange
        var maxConcurrent = _faker.Random.Int(1, 10);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AWS:SES:MaxConcurrentEmails"] = maxConcurrent.ToString()
            })
            .Build();

        // Act
        var act = () => new EmailConcurrencyService(configuration);

        // Assert
        act.Should().NotThrow();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw ArgumentOutOfRangeException when max concurrent is zero")]
    public void Constructor_WhenMaxConcurrentIsZero_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:SES:MaxConcurrentEmails"] = "0" })
            .Build();

        // Act
        var act = () => new EmailConcurrencyService(configuration);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*maxCount*");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw ArgumentOutOfRangeException when max concurrent is negative")]
    public void Constructor_WhenMaxConcurrentIsNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:SES:MaxConcurrentEmails"] = "-1" })
            .Build();

        // Act
        var act = () => new EmailConcurrencyService(configuration);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*initialCount*");
    }

    [Trait("Category", "Unit")]
    [Theory(DisplayName = "Should create instance with valid positive max concurrent values")]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Constructor_WhenMaxConcurrentIsPositive_ShouldCreateSuccessfully(int maxConcurrent)
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AWS:SES:MaxConcurrentEmails"] = maxConcurrent.ToString()
            })
            .Build();

        // Act
        var act = () => new EmailConcurrencyService(configuration);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region AcquireAsync Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should acquire semaphore successfully")]
    public async Task AcquireAsync_WhenCalled_ShouldReturnDisposable()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:SES:MaxConcurrentEmails"] = "3" })
            .Build();
        var sut = new EmailConcurrencyService(configuration);

        // Act
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var disposable = await sut.AcquireAsync(cts.Token);

        // Assert
        disposable.Should().NotBeNull();
        disposable.Should().BeAssignableTo<IDisposable>();

        // Cleanup
        disposable.Dispose();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should release semaphore when disposed")]
    public async Task AcquireAsync_WhenDisposed_ShouldReleaseSemaphore()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:SES:MaxConcurrentEmails"] = "1" })
            .Build();
        var sut = new EmailConcurrencyService(configuration);

        // Act
        var firstAcquire = await sut.AcquireAsync();
        firstAcquire.Dispose();

        var secondAcquire = await sut.AcquireAsync();

        // Assert
        secondAcquire.Should().NotBeNull();

        // Cleanup
        secondAcquire.Dispose();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should allow multiple concurrent acquisitions up to limit")]
    public async Task AcquireAsync_WhenCalledMultipleTimes_ShouldAllowUpToMaxConcurrent()
    {
        // Arrange
        var maxConcurrent = 3;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AWS:SES:MaxConcurrentEmails"] = maxConcurrent.ToString()
            })
            .Build();
        var sut = new EmailConcurrencyService(configuration);
        var disposables = new List<IDisposable>();

        // Act
        for (int i = 0; i < maxConcurrent; i++)
        {
            var disposable = await sut.AcquireAsync();
            disposables.Add(disposable);
        }

        // Assert
        disposables.Should().HaveCount(maxConcurrent);
        disposables.Should().AllBeAssignableTo<IDisposable>();

        // Cleanup
        disposables.ForEach(d => d.Dispose());
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should block when max concurrent limit is reached")]
    public async Task AcquireAsync_WhenMaxConcurrentReached_ShouldBlock()
    {
        // Arrange
        const int maxConcurrent = 2;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AWS:SES:MaxConcurrentEmails"] = maxConcurrent.ToString()
            })
            .Build();
        var sut = new EmailConcurrencyService(configuration);

        var firstAcquire = await sut.AcquireAsync();
        var secondAcquire = await sut.AcquireAsync();

        // Act
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        var act = async () => await sut.AcquireAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        // Cleanup
        firstAcquire.Dispose();
        secondAcquire.Dispose();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should respect cancellation token")]
    public async Task AcquireAsync_WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:SES:MaxConcurrentEmails"] = "1" })
            .Build();

        var sut = new EmailConcurrencyService(configuration);
        var firstAcquire = await sut.AcquireAsync();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var act = async () => await sut.AcquireAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        // Cleanup
        firstAcquire.Dispose();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle concurrent acquisitions from multiple threads")]
    public async Task AcquireAsync_WhenCalledConcurrently_ShouldHandleThreadSafely()
    {
        // Arrange
        var maxConcurrent = 5;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AWS:SES:MaxConcurrentEmails"] = maxConcurrent.ToString()
            })
            .Build();
        var sut = new EmailConcurrencyService(configuration);

        var tasks = new List<Task<IDisposable>>();

        // Act
        for (int i = 0; i < maxConcurrent; i++)
        {
            tasks.Add(Task.Run(async () => await sut.AcquireAsync()));
        }

        var disposables = await Task.WhenAll(tasks);

        // Assert
        disposables.Should().HaveCount(maxConcurrent);
        disposables.Should().AllBeAssignableTo<IDisposable>();

        // Cleanup
        foreach (var disposable in disposables)
            disposable.Dispose();
    }

    #endregion

    #region Dispose Tests

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should not throw when disposing multiple times")]
    public async Task AcquireAsync_WhenDisposedMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["AWS:SES:MaxConcurrentEmails"] = "3" })
            .Build();
        var sut = new EmailConcurrencyService(configuration);
        var disposable = await sut.AcquireAsync();

        // Act
        var act = () =>
        {
            disposable.Dispose();
            disposable.Dispose();
            disposable.Dispose();
        };

        // Assert
        act.Should()
            .Throw<SemaphoreFullException>("Semaphore.Release() lança exceção quando chamado mais vezes que Wait()");
    }

    #endregion
}