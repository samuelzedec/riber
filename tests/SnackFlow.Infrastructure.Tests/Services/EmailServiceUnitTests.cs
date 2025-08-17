using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Services;

namespace SnackFlow.Infrastructure.Tests.Services;

public sealed class EmailServiceUnitTests : BaseTest
{
    private readonly Mock<IAmazonSimpleEmailService> _mockAmazonSimpleEmailService;
    private readonly Mock<ILogger<EmailService>> _mockLogger;
    private readonly EmailService _emailService;

    public EmailServiceUnitTests()
    {
        _mockAmazonSimpleEmailService = new Mock<IAmazonSimpleEmailService>();
        _mockLogger = new Mock<ILogger<EmailService>>();

        _emailService = new EmailService(
            _mockAmazonSimpleEmailService.Object,
            _mockLogger.Object
        );
    }

    [Fact(DisplayName = "Sending email should call AWS SES with correct parameters")]
    public async Task SendAsync_WhenCalled_ShouldCallAwsSesWithCorrectParameters()
    {
        // Arrange
        var to = _faker.Person.Email;
        var subject = _faker.Random.String2(10);
        var body = _faker.Random.String2(100);
        var emailAddres = _faker.Person.Email;
        
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendEmailResponse());
        
        // Act
        var result = async () => await _emailService.SendAsync(to, subject, body, emailAddres);

        // Assert
        await result.Should().NotThrowAsync();
    }

    [Fact(DisplayName = "Sending email should create request with proper structure")]
    public async Task SendAsync_WhenCalled_ShouldCreateRequestWithProperStructure()
    {
        // Arrange
        var to = _faker.Person.Email;
        var subject = _faker.Random.String2(10);
        var body = _faker.Random.String2(100);
        var emailAddress = _faker.Person.Email;
        
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendEmailResponse());
        
        // Act
        await _emailService.SendAsync(to, subject, body, emailAddress);
        
        // Assert
        _mockAmazonSimpleEmailService.Verify(x => x.SendEmailAsync(
            It.Is<SendEmailRequest>(req => 
                req.Source == emailAddress &&
                req.Destination.ToAddresses.Contains(to) &&
                req.Message.Subject.Data == subject &&
                req.Message.Body.Html.Data == body
            ), default), Times.Once);
    }

    [Fact(DisplayName = "Sending email when AWS throws exception should log and rethrow")]
    public async Task SendAsync_WhenAwsThrowsException_ShouldLogAndRethrow()
    {
        // Arrange
        var to = _faker.Person.Email;
        var subject = _faker.Random.String2(10);
        var body = _faker.Random.String2(100);
        var emailAddres = _faker.Person.Email;
        
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Exception Test"));
        
        // Act
        var result = async () => await _emailService.SendAsync(to, subject, body, emailAddres);
        
        // Assert
        await result.Should().ThrowExactlyAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}