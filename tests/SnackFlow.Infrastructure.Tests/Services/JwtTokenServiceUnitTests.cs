using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SnackFlow.Application.Abstractions.Services;
using SnackFlow.Application.DTOs;
using SnackFlow.Domain.Tests;
using SnackFlow.Infrastructure.Services.Authentication;
using SnackFlow.Infrastructure.Settings;

namespace SnackFlow.Infrastructure.Tests.Services;

public sealed class JwtTokenServiceUnitTests : BaseTest
{
    #region Fields and Setup

    private readonly Mock<ICertificateService> _certificateServiceMock;
    private readonly JwtTokenService _tokenService;
    private readonly UserDetailsDTO _userDetailsTest;
    private readonly AccessTokenSettings _accessTokenSettings;
    private readonly RefreshTokenSettings _refreshTokenSettings;

    public JwtTokenServiceUnitTests()
    {
        X509Certificate2 testCertificate = CreateTestCertificate();
        
        _accessTokenSettings = new AccessTokenSettings
        {
            ExpirationInMinutes = 15,
            Key = "test-access-key",
            Password = "test-access-password",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };

        _refreshTokenSettings = new RefreshTokenSettings
        {
            ExpirationInDays = 7,
            Key = "test-refresh-key",
            Password = "test-refresh-password",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };
        
        _userDetailsTest = CreateFaker<UserDetailsDTO>()
            .CustomInstantiator(f => new UserDetailsDTO(
                Id: Guid.CreateVersion7(),
                UserName: f.Internet.UserName(),
                Email: f.Internet.Email(),
                EmailConfirmed: false,
                PhoneNumber: string.Empty,
                SecurityStamp: f.Random.AlphaNumeric(32),
                UserDomainId: Guid.Empty,
                UserDomain: null!,
                Roles: f.Make(2, () => f.Name.JobTitle()).ToList(),
                Claims: [.. f.Make(2, () => new ClaimDTO(
                    Type: f.Random.Word(),
                    Value: f.Random.Word()
                ))]
            ))
            .Generate();


        _certificateServiceMock = new Mock<ICertificateService>();
        Mock<IOptions<AccessTokenSettings>> accessTokenSettingsMock = new();
        Mock<IOptions<RefreshTokenSettings>> refreshTokenSettingsMock = new();

        accessTokenSettingsMock.Setup(x => x.Value).Returns(_accessTokenSettings);
        refreshTokenSettingsMock.Setup(x => x.Value).Returns(_refreshTokenSettings);
        
        _certificateServiceMock
            .Setup(x => x.LoadCertificate(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(testCertificate);

        _tokenService = new JwtTokenService(
            accessTokenSettingsMock.Object,
            refreshTokenSettingsMock.Object,
            _certificateServiceMock.Object);
    }

    #endregion

    #region GenerateToken Tests

    [Fact(DisplayName = "Generating access token should create valid JWT with correct claims")]
    public void GenerateToken_WhenCalled_ShouldCreateValidJwtWithCorrectClaims()
    {
        // Act
        var token = _tokenService.GenerateToken(_userDetailsTest);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Should().NotBeNull();
        jsonToken.Header.Alg.Should().Be(SecurityAlgorithms.RsaSha256);
        jsonToken.Issuer.Should().Be(_accessTokenSettings.Issuer);
        jsonToken.Audiences.Should().Contain(_accessTokenSettings.Audience);
        
        jsonToken.ValidTo.Should().BeCloseTo(
            DateTime.UtcNow.AddMinutes(_accessTokenSettings.ExpirationInMinutes), 
            TimeSpan.FromSeconds(5));
        
        jsonToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == _userDetailsTest.Id.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == _userDetailsTest.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == _userDetailsTest.UserName);
        jsonToken.Claims.Should().Contain(c => c.Type == "securityStamp" && c.Value == _userDetailsTest.SecurityStamp);
        
        foreach (var role in _userDetailsTest.Roles)
        {
            jsonToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == role);
        }
        
        foreach (var claim in _userDetailsTest.Claims)
        {
            jsonToken.Claims.Should().Contain(c => c.Type == claim.Type && c.Value == claim.Value);
        }
    }

    [Fact(DisplayName = "Generating access token should call certificate service with correct parameters")]
    public void GenerateToken_WhenCalled_ShouldCallCertificateServiceWithCorrectParameters()
    {
        // Act
        _tokenService.GenerateToken(_userDetailsTest);

        // Assert
        _certificateServiceMock.Verify(
            x => x.LoadCertificate(_accessTokenSettings.Key, _accessTokenSettings.Password),
            Times.Once);
    }

    [Fact(DisplayName = "Generating access token with empty roles should not include role claims")]
    public void GenerateToken_WhenUserHasNoRoles_ShouldNotIncludeRoleClaims()
    {
        // Arrange
        var userWithoutRoles = _userDetailsTest with { Roles = [] };
        
        // Act
        var token = _tokenService.GenerateToken(userWithoutRoles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Claims.Should().NotContain(c => c.Type == "role");
    }

    [Fact(DisplayName = "Generating access token with empty claims should not include custom claims")]
    public void GenerateToken_WhenUserHasNoClaims_ShouldNotIncludeCustomClaims()
    {
        // Arrange
        var userWithoutClaims = _userDetailsTest with { Claims = [] };

        // Act
        var token = _tokenService.GenerateToken(userWithoutClaims);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
   
        jsonToken.Claims.Count().Should().Be(10 + userWithoutClaims.Roles.Count);
    }

    #endregion

    #region GenerateRefreshToken Tests

    [Fact(DisplayName = "Generating refresh token should create valid JWT with correct claims")]
    public void GenerateRefreshToken_WhenCalled_ShouldCreateValidJwtWithCorrectClaims()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var securityStamp = _faker.Random.AlphaNumeric(32);

        // Act
        var token = _tokenService.GenerateRefreshToken(userId, securityStamp);

        // Assert
        token.Should().NotBeNullOrEmpty();
    
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
    
        jsonToken.Should().NotBeNull();
        jsonToken.Header.Alg.Should().Be(SecurityAlgorithms.RsaSha256);
        jsonToken.Issuer.Should().Be(_refreshTokenSettings.Issuer);
        jsonToken.Audiences.Should().Contain(_refreshTokenSettings.Audience);
    
        jsonToken.ValidTo.Should().BeCloseTo(
            DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationInDays), 
            TimeSpan.FromSeconds(5));
    
        jsonToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == "tokenType" && c.Value == "refresh");
        jsonToken.Claims.Should().Contain(c => c.Type == "securityStamp" && c.Value == securityStamp);
    
        var customClaims = jsonToken.Claims.Where(c => 
            c.Type != "nbf" && c.Type != "exp" && c.Type != "iat" && 
            c.Type != "iss" && c.Type != "aud");
        customClaims.Count().Should().Be(3);
    }

    [Fact(DisplayName = "Generating refresh token should call certificate service with correct parameters")]
    public void GenerateRefreshToken_WhenCalled_ShouldCallCertificateServiceWithCorrectParameters()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var securityStamp = _faker.Random.AlphaNumeric(32);

        // Act
        _tokenService.GenerateRefreshToken(userId, securityStamp);

        // Assert
        _certificateServiceMock.Verify(
            x => x.LoadCertificate(_refreshTokenSettings.Key, _refreshTokenSettings.Password),
            Times.Once);
    }

    [Fact(DisplayName = "Generating refresh token should use different certificate than access token")]
    public void GenerateRefreshToken_WhenCalled_ShouldUseDifferentCertificateThanAccessToken()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var securityStamp = _faker.Random.AlphaNumeric(32);

        // Act
        _tokenService.GenerateToken(_userDetailsTest);
        _tokenService.GenerateRefreshToken(userId, securityStamp);

        // Assert
        _certificateServiceMock.Verify(
            x => x.LoadCertificate(_accessTokenSettings.Key, _accessTokenSettings.Password),
            Times.Once);
        
        _certificateServiceMock.Verify(
            x => x.LoadCertificate(_refreshTokenSettings.Key, _refreshTokenSettings.Password),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static X509Certificate2 CreateTestCertificate()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var request = new CertificateRequest(
            "cn=test", rsa, System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        
        return request.CreateSelfSigned(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(365));
    }

    #endregion
}