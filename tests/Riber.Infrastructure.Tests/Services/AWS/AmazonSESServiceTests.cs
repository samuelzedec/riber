using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using FluentAssertions;
using Moq;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.AWS.Email;

namespace Riber.Infrastructure.Tests.Services.AWS;

public sealed class AmazonSESServiceTests : BaseTest
{
    private readonly Mock<IAmazonSimpleEmailService> _mockAmazonSimpleEmailService;
    private readonly AmazonSESService _sut;
    private readonly string _to;
    private readonly string _subject;
    private readonly string _body;
    private readonly string _emailAddress;

    public AmazonSESServiceTests()
    {
        _mockAmazonSimpleEmailService = new Mock<IAmazonSimpleEmailService>();
        _sut = new AmazonSESService(_mockAmazonSimpleEmailService.Object);

        _to = _faker.Internet.Email();
        _subject = _faker.Lorem.Sentence();
        _body = _faker.Lorem.Paragraphs();
        _emailAddress = _faker.Internet.Email();
    }

    [Fact(DisplayName = "Should send email successfully when AWS SES returns success")]
    public async Task SendAsync_WhenAwsReturnsSuccess_ShouldCompleteSuccessfully()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
            .ReturnsAsync(new SendEmailResponse { MessageId = _faker.Random.Guid().ToString() });

        // Act
        var act = async () => await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact(DisplayName = "Should call AWS SES exactly once")]
    public async Task SendAsync_WhenCalled_ShouldCallAwsSesOnce()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), default),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should create request with correct source email address")]
    public async Task SendAsync_WhenCalled_ShouldSetCorrectSourceEmail()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req => req.Source == _emailAddress),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should create request with correct destination email")]
    public async Task SendAsync_WhenCalled_ShouldSetCorrectDestinationEmail()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), default))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req =>
                    req.Destination.ToAddresses.Count == 1 &&
                    req.Destination.ToAddresses.Contains(_to)
                ),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should create request with correct subject")]
    public async Task SendAsync_WhenCalled_ShouldSetCorrectSubject()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req =>
                    req.Message.Subject.Data == _subject
                ),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should create request with HTML body")]
    public async Task SendAsync_WhenCalled_ShouldSetHtmlBody()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req =>
                    req.Message.Body.Html != null &&
                    req.Message.Body.Html.Data == _body
                ),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should create request with all parameters correctly")]
    public async Task SendAsync_WhenCalled_ShouldCreateCompleteRequest()
    {
        // Arrange
        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req =>
                    req.Source == _emailAddress &&
                    req.Destination.ToAddresses.Contains(_to) &&
                    req.Destination.ToAddresses.Count == 1 &&
                    req.Message.Subject.Data == _subject &&
                    req.Message.Body.Html.Data == _body
                ),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should throw AmazonSimpleEmailServiceException when AWS service fails")]
    public async Task SendAsync_WhenAwsServiceFails_ShouldThrowAmazonException()
    {
        // Arrange
        var expectedException = new AmazonSimpleEmailServiceException("AWS SES service error");

        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<AmazonSimpleEmailServiceException>()
            .WithMessage("AWS SES service error");
    }

    [Fact(DisplayName = "Should throw MessageRejectedException when email is rejected")]
    public async Task SendAsync_WhenEmailIsRejected_ShouldThrowMessageRejectedException()
    {
        // Arrange
        var expectedException = new MessageRejectedException("Email rejected by AWS SES");

        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<MessageRejectedException>()
            .WithMessage("Email rejected by AWS SES");
    }

    [Fact(DisplayName = "Should throw MailFromDomainNotVerifiedException when sender domain not verified")]
    public async Task SendAsync_WhenSenderNotVerified_ShouldThrowMailFromDomainNotVerifiedException()
    {
        // Arrange
        var expectedException = new MailFromDomainNotVerifiedException("Sender domain not verified");

        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _sut.SendAsync(_to, _subject, _body, _emailAddress);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<MailFromDomainNotVerifiedException>()
            .WithMessage("Sender domain not verified");
    }

    [Fact(DisplayName = "Should handle long email body")]
    public async Task SendAsync_WhenBodyIsLong_ShouldSendSuccessfully()
    {
        // Arrange
        var longBody = string.Join("", Enumerable.Repeat(_faker.Lorem.Paragraphs(10), 10));

        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        var act = async () => await _sut.SendAsync(_to, _subject, longBody, _emailAddress);

        // Assert
        await act.Should().NotThrowAsync();

        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req => req.Message.Body.Html.Data == longBody),
                CancellationToken.None
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = "Should handle special characters in subject and body")]
    public async Task SendAsync_WhenContainsSpecialCharacters_ShouldSendSuccessfully()
    {
        // Arrange
        var specialSubject = "Test <HTML> & \"Quotes\" 'Apostrophe' €";
        var specialBody = "<h1>HTML Content</h1><p>With & special © characters ™</p>";

        _mockAmazonSimpleEmailService
            .Setup(x => x.SendEmailAsync(It.IsAny<SendEmailRequest>(), CancellationToken.None
            ))
            .ReturnsAsync(new SendEmailResponse());

        // Act
        await _sut.SendAsync(_to, specialSubject, specialBody, _emailAddress);

        // Assert
        _mockAmazonSimpleEmailService.Verify(
            x => x.SendEmailAsync(
                It.Is<SendEmailRequest>(req =>
                    req.Message.Subject.Data == specialSubject &&
                    req.Message.Body.Html.Data == specialBody
                ),
                CancellationToken.None
            ),
            Times.Once
        );
    }
}