using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Riber.Application.Abstractions.Services.Email;
using Riber.Infrastructure.BackgroundJobs;

namespace Riber.Infrastructure.Tests.BackgroundJobs;

public class SendingEmailJobTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IEmailTemplateRender> _mockTemplateRender;
    private readonly Mock<IEmailConcurrencyService> _mockConcurrencyService;
    private readonly Mock<ILogger<SendingEmailJob>> _mockLogger;
    private readonly Mock<IJobExecutionContext> _mockJobContext;
    private readonly Mock<IDisposable> _mockDisposable;
    private readonly JobDataMap _jobDataMap;
    private readonly SendingEmailJob _job;
    private readonly Faker _faker;

    public SendingEmailJobTests()
    {
        _mockEmailService = new Mock<IEmailService>();
        _mockTemplateRender = new Mock<IEmailTemplateRender>();
        _mockConcurrencyService = new Mock<IEmailConcurrencyService>();
        _mockLogger = new Mock<ILogger<SendingEmailJob>>();
        _mockJobContext = new Mock<IJobExecutionContext>();
        Mock<ITrigger> mockTrigger = new();
        _mockDisposable = new Mock<IDisposable>();
        _jobDataMap = [];
        _faker = new Faker();

        _job = new SendingEmailJob(
            _mockEmailService.Object,
            _mockTemplateRender.Object,
            _mockConcurrencyService.Object,
            _mockLogger.Object
        );

        _mockJobContext.Setup(x => x.Trigger).Returns(mockTrigger.Object);
        _mockJobContext.Setup(x => x.CancellationToken).Returns(CancellationToken.None);
        mockTrigger.Setup(x => x.JobDataMap).Returns(_jobDataMap);
        _mockConcurrencyService.Setup(x => x.AcquireAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_mockDisposable.Object);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should send email successfully with valid data")]
    public async Task Execute_WithValidData_ShouldSendEmailSuccessfully()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var to = _faker.Internet.Email();
        var subject = _faker.Lorem.Sentence();
        var templateProcessed = _faker.Lorem.Paragraph();

        var emailPayload = new JObject
        {
            ["to"] = to,
            ["subject"] = subject,
            ["template"] = "welcome"
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(templateProcessed);

        _mockEmailService
            .Setup(x => x.SendAsync(to, subject, templateProcessed, emailAddress))
            .Returns(Task.CompletedTask);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockEmailService.Verify(
            x => x.SendAsync(to, subject, templateProcessed, emailAddress),
            Times.Once
        );

        _mockTemplateRender.Verify(
            x => x.GetTemplateAsync(It.Is<JObject>(j => 
                j["to"]!.ToString() == to && 
                j["subject"]!.ToString() == subject)),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should acquire concurrency lock before processing")]
    public async Task Execute_ShouldAcquireConcurrencyLockBeforeProcessing()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var emailPayload = new JObject
        {
            ["to"] = _faker.Internet.Email(),
            ["subject"] = _faker.Lorem.Sentence()
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(_faker.Lorem.Paragraph());

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockConcurrencyService.Verify(
            x => x.AcquireAsync(CancellationToken.None),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should dispose concurrency lock after execution")]
    public async Task Execute_ShouldDisposeConcurrencyLockAfterExecution()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var emailPayload = new JObject
        {
            ["to"] = _faker.Internet.Email(),
            ["subject"] = _faker.Lorem.Sentence()
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(_faker.Lorem.Paragraph());

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockDisposable.Verify(x => x.Dispose(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should render email template with correct payload")]
    public async Task Execute_ShouldRenderEmailTemplateWithCorrectPayload()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var to = _faker.Internet.Email();
        var subject = _faker.Lorem.Sentence();
        var templateName = "password-reset";

        var emailPayload = new JObject
        {
            ["to"] = to,
            ["subject"] = subject,
            ["template"] = templateName,
            ["data"] = new JObject
            {
                ["resetLink"] = _faker.Internet.Url()
            }
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        var templateProcessed = _faker.Lorem.Paragraph();
        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(templateProcessed);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockTemplateRender.Verify(
            x => x.GetTemplateAsync(It.Is<JObject>(j => 
                j["template"]!.ToString() == templateName)),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log error when email sending fails")]
    public async Task Execute_WhenEmailSendingFails_ShouldLogError()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var emailPayload = new JObject
        {
            ["to"] = _faker.Internet.Email(),
            ["subject"] = _faker.Lorem.Sentence()
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        var expectedException = new Exception("SMTP connection failed");
        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(_faker.Lorem.Paragraph());

        _mockEmailService
            .Setup(x => x.SendAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .ThrowsAsync(expectedException);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => 
                    v.ToString()!.Contains("Permanent failure sending email") &&
                    v.ToString()!.Contains("Will NOT retry")),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should not throw exception when sending fails")]
    public async Task Execute_WhenEmailSendingFails_ShouldNotThrowException()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var emailPayload = new JObject
        {
            ["to"] = _faker.Internet.Email(),
            ["subject"] = _faker.Lorem.Sentence()
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(_faker.Lorem.Paragraph());

        _mockEmailService
            .Setup(x => x.SendAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMTP error"));

        // Act
        var act = async () => await _job.Execute(_mockJobContext.Object);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should log error when template rendering fails")]
    public async Task Execute_WhenTemplateRenderingFails_ShouldLogError()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var emailPayload = new JObject
        {
            ["to"] = _faker.Internet.Email(),
            ["subject"] = _faker.Lorem.Sentence()
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        var expectedException = new Exception("Template not found");
        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ThrowsAsync(expectedException);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
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
    [Fact(DisplayName = "Should dispose concurrency lock even when exception occurs")]
    public async Task Execute_WhenExceptionOccurs_ShouldStillDisposeConcurrencyLock()
    {
        // Arrange
        var emailAddress = _faker.Internet.Email();
        var emailPayload = new JObject
        {
            ["to"] = _faker.Internet.Email(),
            ["subject"] = _faker.Lorem.Sentence()
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ThrowsAsync(new Exception("Template error"));

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockDisposable.Verify(x => x.Dispose(), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should extract correct values from JobDataMap")]
    public async Task Execute_ShouldExtractCorrectValuesFromJobDataMap()
    {
        // Arrange
        var emailAddress = "sender@example.com";
        var to = "recipient@example.com";
        var subject = "Test Subject";

        var emailPayload = new JObject
        {
            ["to"] = to,
            ["subject"] = subject
        };

        _jobDataMap.Put("emailAddress", emailAddress);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        var templateProcessed = "<html>Test</html>";
        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(templateProcessed);

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockEmailService.Verify(
            x => x.SendAsync(to, subject, templateProcessed, emailAddress),
            Times.Once
        );
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should pass emailAddress as sender parameter")]
    public async Task Execute_ShouldPassEmailAddressAsSenderParameter()
    {
        // Arrange
        var senderEmail = "noreply@company.com";
        var recipientEmail = _faker.Internet.Email();
        var subject = _faker.Lorem.Sentence();

        var emailPayload = new JObject
        {
            ["to"] = recipientEmail,
            ["subject"] = subject
        };

        _jobDataMap.Put("emailAddress", senderEmail);
        _jobDataMap.Put("emailPayload", JsonConvert.SerializeObject(emailPayload));

        _mockTemplateRender
            .Setup(x => x.GetTemplateAsync(It.IsAny<JObject>()))
            .ReturnsAsync(_faker.Lorem.Paragraph());

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        _mockEmailService.Verify(
            x => x.SendAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                senderEmail),
            Times.Once
        );
    }
}