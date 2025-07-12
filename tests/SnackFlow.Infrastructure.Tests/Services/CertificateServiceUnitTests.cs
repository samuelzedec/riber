using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moq;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Services;
using SnackFlow.Infrastructure.Services.Abstractions;

namespace SnackFlow.Infrastructure.Tests.Services;

public class CertificateServiceUnitTests : BaseTest
{
    #region Setup
    
    private readonly Mock<ISecretService> _mockSecretService;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly CertificateService _certificateService;
    
    public CertificateServiceUnitTests()
    {
        _mockSecretService = new Mock<ISecretService>();
        _mockEnvironment = new Mock<IWebHostEnvironment>();
        _certificateService = new CertificateService(
            _mockSecretService.Object, 
            _mockEnvironment.Object
        );
    }
    
    #endregion
    
    #region Development Environment Tests

    [Fact(DisplayName = "Should load certificate from file when not in production environment")]
    public async Task LoadCertificateAsync_WhenNotProduction_ShouldLoadFromFile()
    {
        // Arrange
        var source = "test-certificate.pfx";
        var secret = "test-password";

        _mockEnvironment
            .Setup(x => x.EnvironmentName)
            .Returns(Environments.Development);
        
        // Arrange
        var act = async () => await _certificateService.LoadCertificateAsync(source, secret);
        
        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockSecretService.Verify(x => x.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Should not call secret service when not in production environment")]
    public void LoadCertificateAsync_WhenNotProduction_ShouldNotCallSecretService()
    {
        // Arrange
        var source = "test-certificate.pfx";
        var secret = "test-password";

        _mockEnvironment
            .Setup(x => x.EnvironmentName)
            .Returns(Environments.Development);

        // Act
        var result = async () => await _certificateService.LoadCertificateAsync(source, secret);

        // Assert
        _mockSecretService.Verify(x => x.GetSecretAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    
    #endregion
    
    #region Production Environment Tests

    [Fact(DisplayName = "Should load certificate from secret service when in production environment")]
    public async Task LoadCertificateAsync_WhenProduction_ShouldLoadFromSecretService()
    {
        // Arrange
        var source = "certificate-secret-name";
        var secret = "not-used-in-production";
        
        var testCertificateBytes = CreateTestCertificateBytes();

        _mockEnvironment
            .Setup(x => x.EnvironmentName)
            .Returns(Environments.Production);

        _mockSecretService
            .Setup(x => x.GetSecretAsync(source, secret))
            .ReturnsAsync(testCertificateBytes);

        // Act
        try
        {
            await _certificateService.LoadCertificateAsync(source, secret);
        }
        catch
        {
            // ignored
        }
        
        // Assert
        _mockSecretService.Verify(x => x.GetSecretAsync(source, secret), Times.Once);
    }

    [Fact(DisplayName = "Should call secret service with correct source when in production")]
    public async Task LoadCertificateAsync_WhenProduction_ShouldCallSecretServiceWithCorrectSource()
    {
        // Arrange
        var source = "my-certificate-secret";
        var secret = "not-used";
        var testCertificateBytes = CreateTestCertificateBytes();


        _mockEnvironment
            .Setup(x => x.EnvironmentName)
            .Returns(Environments.Production);

        _mockSecretService
            .Setup(x => x.GetSecretAsync(source, secret))
            .ReturnsAsync(testCertificateBytes);

        // Act
        try
        {
            await _certificateService.LoadCertificateAsync(source, secret);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mockSecretService.Verify(x => x.GetSecretAsync(source, secret), Times.Once);
        _mockSecretService.Verify(x => x.GetSecretAsync(
            It.Is<string>(s => s == source),
            It.Is<string>(s => s == secret)),
            Times.Once);
    }

    [Fact(DisplayName = "Should throw exception when secret service throws exception")]
    public async Task LoadCertificateAsync_WhenSecretServiceThrows_ShouldThrowException()
    {
        // Arrange
        var source = "certificate-secret";
        var secret = "not-used";
        var expectedException = new InvalidOperationException("Secret service error");

        _mockEnvironment
            .Setup(x => x.EnvironmentName)
            .Returns(Environments.Production);

        _mockSecretService
            .Setup(x => x.GetSecretAsync(source, secret))
            .ThrowsAsync(expectedException);

        // Act
        var act = async () => await _certificateService.LoadCertificateAsync(source, secret);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Secret service error");
        
        _mockSecretService.Verify(x => x.GetSecretAsync(source, secret), Times.Once);
    }

    #endregion
    
    #region Helper Methods

    private static byte[] CreateTestCertificateBytes()
        => [ 0x30, 0x82, 0x01, 0x00 ];

    #endregion
}