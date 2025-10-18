using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Behaviors;
using Riber.Application.Exceptions;
using Riber.Application.Tests.Behaviors.TestModels;
using Riber.Domain.Tests;
using System.Diagnostics;
using Mediator;

namespace Riber.Application.Tests.Behaviors;

public sealed class LoggingBehaviorTests : BaseTest
{
    private readonly Mock<ILogger<RequestTest>> _mockLogger;
    private readonly RequestTest _request;
    private readonly ResponseTest _response;

    public LoggingBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<RequestTest>>();

        _request = CreateFaker<RequestTest>()
            .CustomInstantiator(f => new RequestTest(
                f.Person.FullName, 
                f.Random.Int(20, 50)
            ));

        _response = CreateFaker<ResponseTest>()
            .CustomInstantiator(f => new ResponseTest(f.Person.FullName));
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log request start with correct message name")]
    public async Task Handle_WhenRequestStarts_ShouldLogRequestName()
    {
        // Arrange
        var sut = CreateSut();
        var expectedMessageName = nameof(RequestTest);

        // Act
        await sut.Handle(_request, CreateSuccessfulHandler(), CancellationToken.None);

        // Assert
        VerifyLogMessage(
            LogLevel.Information,
            $"Handling: {expectedMessageName}",
            Times.Once()
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log request completion with elapsed time")]
    public async Task Handle_WhenRequestCompletes_ShouldLogCompletionWithDuration()
    {
        // Arrange
        var sut = CreateSut();
        var expectedMessageName = nameof(RequestTest);

        // Act
        await sut.Handle(_request, CreateSuccessfulHandler(), CancellationToken.None);

        // Assert
        VerifyLogContains(
            LogLevel.Information,
            expectedMessageName,
            "completed in",
            "ms",
            Times.Once()
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw RequestTimeoutException when request is cancelled")]
    public async Task Handle_WhenCancellationRequested_ShouldThrowRequestTimeoutException()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act
        var act = async () => await sut.Handle(
            _request,
            CreateCancelledHandler(cts.Token),
            cts.Token
        );

        // Assert
        await act.Should()
            .ThrowExactlyAsync<RequestTimeoutException>()
            .WithMessage($"*{nameof(RequestTest)}*");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log warning when request is cancelled")]
    public async Task Handle_WhenCancellationRequested_ShouldLogWarningWithElapsedTime()
    {
        // Arrange
        var sut = CreateSut();
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var expectedMessageName = nameof(RequestTest);

        // Act
        try
        {
            await sut.Handle(_request, CreateCancelledHandler(cts.Token), cts.Token);
        }
        catch (RequestTimeoutException)
        {
            // Expected exception
        }

        // Assert
        VerifyLogContains(
            LogLevel.Warning,
            expectedMessageName,
            "cancelled after",
            Times.Once()
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log error and re-throw when handler throws exception")]
    public async Task Handle_WhenHandlerThrowsException_ShouldLogErrorAndReThrow()
    {
        // Arrange
        var sut = CreateSut();
        var expectedException = new InvalidOperationException("Test error");
        var expectedMessageName = nameof(RequestTest);

        // Act
        var act = async () => await sut.Handle(
            _request,
            CreateFailingHandler(expectedException),
            CancellationToken.None
        );

        // Assert
        await act.Should()
            .ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage("Test error");

        VerifyLogContains(
            LogLevel.Error,
            expectedMessageName,
            "failed after",
            "ms",
            Times.Once()
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should preserve exception details when logging error")]
    public async Task Handle_WhenHandlerThrowsException_ShouldLogExceptionDetails()
    {
        // Arrange
        var sut = CreateSut();
        var expectedException = new InvalidOperationException("Test error");

        // Act
        try
        {
            await sut.Handle(
                _request,
                CreateFailingHandler(expectedException),
                CancellationToken.None
            );
        }
        catch (InvalidOperationException)
        {
            // Expected exception
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                expectedException, // Verifica que a exceção original foi passada
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should create activity with correct name")]
    public async Task Handle_WhenRequestProcessed_ShouldCreateActivityWithCorrectName()
    {
        // Arrange
        var sut = CreateSut();
        Activity? capturedActivity = null;

        // Captura a Activity criada
        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == "Riber.Application";
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        listener.ActivityStarted = activity => capturedActivity = activity;
        ActivitySource.AddActivityListener(listener);

        // Act
        await sut.Handle(_request, CreateSuccessfulHandler(), CancellationToken.None);

        // Assert
        capturedActivity.Should().NotBeNull();
        capturedActivity!.DisplayName.Should().Be($"Mediator.{nameof(RequestTest)}");
    }

    #region Helper Methods

    private LoggingBehavior<RequestTest, ResponseTest> CreateSut()
        => new(_mockLogger.Object);

    private MessageHandlerDelegate<RequestTest, ResponseTest> CreateSuccessfulHandler()
        => (_, _) => ValueTask.FromResult(_response);

    private static MessageHandlerDelegate<RequestTest, ResponseTest> CreateCancelledHandler(
        CancellationToken token)
        => (_, _) => throw new OperationCanceledException(token);

    private static MessageHandlerDelegate<RequestTest, ResponseTest> CreateFailingHandler(
        Exception exception)
        => (_, _) => throw exception;

    private void VerifyLogMessage(LogLevel level, string message, Times times)
    {
        _mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            times
        );
    }

    private void VerifyLogContains(
        LogLevel level,
        string message,
        string additionalContent,
        Times times)
    {
        _mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains(message) &&
                    v.ToString()!.Contains(additionalContent)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            times
        );
    }

    private void VerifyLogContains(
        LogLevel level,
        string message,
        string content1,
        string content2,
        Times times)
    {
        _mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains(message) &&
                    v.ToString()!.Contains(content1) &&
                    v.ToString()!.Contains(content2)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            times
        );
    }

    #endregion
}