using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SnackFlow.Application.Behaviors;
using SnackFlow.Application.Exceptions;
using SnackFlow.Application.Tests.Behaviors.TestModels;
using SnackFlow.Domain.Tests;

namespace SnackFlow.Application.Tests.Behaviors;

public sealed class LoggingBehaviorUnitTests : BaseTest
{
    private readonly Mock<ILogger<RequestTest>> _mockLogger;
    private readonly LoggingBehavior<RequestTest, ResponseTest> _loggingBehavior;
    private readonly RequestTest _request;
    private readonly ResponseTest _response;

    public LoggingBehaviorUnitTests()
    {
        _mockLogger = new Mock<ILogger<RequestTest>>();
        _loggingBehavior = new LoggingBehavior<RequestTest, ResponseTest>(_mockLogger.Object);

        _request = CreateFaker<RequestTest>().CustomInstantiator(f
            => new RequestTest(f.Person.FullName, f.Random.Int(20, 50)));

        _response = CreateFaker<ResponseTest>().CustomInstantiator(f
            => new ResponseTest(f.Person.FullName));
    }

    [Fact(DisplayName = "Should log request start and completion successfully")]
    public async Task Handle_WhenRequestCompletes_ShouldLogRequestStartAndCompletion()
    {
        // Act
        await _loggingBehavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        _mockLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
        
        _mockLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("processed in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
    
    [Fact(DisplayName = "Should throw RequestTimeoutException when cancellation token is cancelled")]
    public async Task Handle_WhenCancellationTokenIsCancelled_ShouldThrowRequestTimeoutException()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        
        // Act
        var exception = async () => await _loggingBehavior.Handle(
            _request, 
            (_, _) => throw new OperationCanceledException(cts.Token), 
            cts.Token
        );
            
        // Assert
        await exception.Should().ThrowExactlyAsync<RequestTimeoutException>();
        
        _mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("timed out")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
    
    [Fact(DisplayName = "Should log error and re-throw when handler throws exception")]
    public async Task Handle_WhenHandlerThrowsException_ShouldLogErrorAndReThrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test error");
        
        // Act
        var exception = async () => await _loggingBehavior.Handle(
            _request, 
            (_, _) => throw expectedException, 
            CancellationToken.None
        );
            
        // Assert
        await exception.Should().ThrowExactlyAsync<InvalidOperationException>().WithMessage("Test error");
        
        _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error while handling request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}