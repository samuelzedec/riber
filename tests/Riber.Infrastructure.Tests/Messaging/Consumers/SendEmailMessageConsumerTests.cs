using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Riber.Application.Abstractions.Services.Email;
using Riber.Application.Messages;
using Riber.Infrastructure.Messaging.Consumers;

namespace Riber.Infrastructure.Tests.Messaging.Consumers;

public sealed class SendEmailMessageConsumerTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IEmailTemplateRender> _mockTemplateRender;
    private readonly Mock<IEmailConcurrencyService> _mockConcurrencyService;
    private readonly Mock<ILogger<SendEmailMessageConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<SendEmailMessage>> _mockConsumeContext;
    private readonly SendEmailMessageConsumer _consumer;

    public SendEmailMessageConsumerTests()
    {
        _mockEmailService = new Mock<IEmailService>();
        _mockTemplateRender = new Mock<IEmailTemplateRender>();
        _mockConcurrencyService = new Mock<IEmailConcurrencyService>();
        _mockLogger = new Mock<ILogger<SendEmailMessageConsumer>>();
        _mockConsumeContext = new Mock<ConsumeContext<SendEmailMessage>>();

        _consumer = new SendEmailMessageConsumer(
            _mockEmailService.Object,
            _mockTemplateRender.Object,
            _mockConcurrencyService.Object,
            _mockLogger.Object
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should send email successfully")]
    public async Task Consume_WithValidMessage_ShouldSendEmailSuccessfully()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: new Dictionary<string, object?> { { "name", "John" } }
        );

        const string renderedHtml = "<html><body>Test Email</body></html>";
        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .ReturnsAsync(renderedHtml);

        _mockEmailService
            .Setup(x => x.SendAsync(
                message.To,
                message.Subject,
                renderedHtml,
                message.From))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockConcurrencyService.Verify(
            x => x.AcquireAsync(cancellationToken),
            Times.Once
        );

        _mockTemplateRender.Verify(
            x => x.GetTemplateAsync(message.TemplatePath, message.Model),
            Times.Once
        );

        _mockEmailService.Verify(
            x => x.SendAsync(message.To, message.Subject, renderedHtml, message.From),
            Times.Once
        );

        mockDisposable.Verify(x => x.Dispose(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should log information when sending email")]
    public async Task Consume_WhenSendingEmail_ShouldLogInformation()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: new Dictionary<string, object?> { { "name", "John" } }
        );

        const string renderedHtml = "<html><body>Test Email</body></html>";
        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .ReturnsAsync(renderedHtml);

        _mockEmailService
            .Setup(x => x.SendAsync(message.To, message.Subject, renderedHtml, message.From))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Sending email to {message.To}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Successfully sent email to {message.To}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should acquire concurrency lock before sending")]
    public async Task Consume_ShouldAcquireConcurrencyLockBeforeSending()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: []
        );

        const string renderedHtml = "<html><body>Test</body></html>";
        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();
        var callOrder = new List<string>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .Callback(() => callOrder.Add("acquire"))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .Callback(() => callOrder.Add("render"))
            .ReturnsAsync(renderedHtml);

        _mockEmailService
            .Setup(x => x.SendAsync(message.To, message.Subject, renderedHtml, message.From))
            .Callback(() => callOrder.Add("send"))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        callOrder.Should().ContainInOrder("acquire", "render", "send");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle template render exception gracefully")]
    public async Task Consume_WhenTemplateRenderFails_ShouldLogErrorAndNotThrow()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: []
        );

        var expectedException = new InvalidOperationException("Template not found");
        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Permanent failure sending email")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );

        _mockEmailService.Verify(
            x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should handle email service exception gracefully")]
    public async Task Consume_WhenEmailServiceFails_ShouldLogErrorAndNotThrow()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: []
        );

        const string renderedHtml = "<html><body>Test</body></html>";
        var expectedException = new InvalidOperationException("Email service unavailable");
        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .ReturnsAsync(renderedHtml);

        _mockEmailService
            .Setup(x => x.SendAsync(message.To, message.Subject, renderedHtml, message.From))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        await act.Should().NotThrowAsync();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Permanent failure sending email")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should dispose concurrency lock even when exception occurs")]
    public async Task Consume_WhenExceptionOccurs_ShouldDisposeConcurrencyLock()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: []
        );

        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        mockDisposable.Verify(x => x.Dispose(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should use correct cancellation token")]
    public async Task Consume_ShouldUseCancellationTokenFromContext()
    {
        // Arrange
        var message = new SendEmailMessage(
            From: "from@example.com",
            To: "to@example.com",
            Subject: "Test Subject",
            TemplatePath: "/templates/test.html",
            Model: []
        );

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, message.Model))
            .ReturnsAsync("<html></html>");

        _mockEmailService
            .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockConcurrencyService.Verify(
            x => x.AcquireAsync(cancellationToken),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Consume should render template with correct model")]
    public async Task Consume_ShouldRenderTemplateWithCorrectModel()
    {
        // Arrange
        var model = new Dictionary<string, object?>
        {
            { "userName", "John Doe" },
            { "resetLink", "https://example.com/reset" }
        };

        var message = new SendEmailMessage(
            From: "noreply@example.com",
            To: "user@example.com",
            Subject: "Password Reset",
            TemplatePath: "/templates/password-reset.html",
            Model: model
        );

        const string renderedHtml = "<html><body>Reset your password</body></html>";
        var cancellationToken = CancellationToken.None;
        var mockDisposable = new Mock<IDisposable>();

        _mockConsumeContext.Setup(x => x.Message).Returns(message);
        _mockConsumeContext.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _mockConcurrencyService
            .Setup(x => x.AcquireAsync(cancellationToken))
            .ReturnsAsync(mockDisposable.Object);

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(message.TemplatePath, model))
            .ReturnsAsync(renderedHtml);

        _mockEmailService
            .Setup(x => x.SendAsync(message.To, message.Subject, renderedHtml, message.From))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_mockConsumeContext.Object);

        // Assert
        _mockTemplateRender.Verify(
            x => x.GetTemplateAsync(message.TemplatePath, model),
            Times.Once
        );

        _mockEmailService.Verify(
            x => x.SendAsync(message.To, message.Subject, renderedHtml, message.From),
            Times.Once
        );
    }
}

