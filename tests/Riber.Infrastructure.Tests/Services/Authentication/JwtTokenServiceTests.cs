using System.IdentityModel.Tokens.Jwt;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Riber.Application.Dtos.Auth;
using Riber.Application.Dtos.User;
using Riber.Domain.Entities.User;
using Riber.Domain.Enums;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.Authentication;
using Riber.Infrastructure.Settings;

namespace Riber.Infrastructure.Tests.Services.Authentication;

public sealed class JwtTokenServiceTests : BaseTest
{
    #region Fields and Setup

    private readonly JwtTokenService _tokenService;
    private readonly UserDetailsDto _userDetailsTest;
    private readonly AccessTokenSettings _accessTokenSettings;
    private readonly RefreshTokenSettings _refreshTokenSettings;

    public JwtTokenServiceTests()
    {
        _accessTokenSettings = new AccessTokenSettings
        {
            ExpirationInMinutes = 15,
            SecretKey = "super-secret-key-for-testing-that-is-long-enough-256-bits",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };

        _refreshTokenSettings = new RefreshTokenSettings
        {
            ExpirationInDays = 7,
            SecretKey = "another-super-secret-key-for-refresh-tokens-256-bits",
            Issuer = "test-refresh-issuer",
            Audience = "test-refresh-audience"
        };
        
        var userDomain = User.Create(
            _faker.Name.FullName(),
            _faker.Person.Cpf(),
            BusinessPosition.Owner,
            Guid.CreateVersion7()
        );
        
        _userDetailsTest = CreateFaker<UserDetailsDto>()
            .CustomInstantiator(f => new UserDetailsDto(
                Id: Guid.CreateVersion7(),
                UserName: f.Internet.UserName(),
                Email: f.Internet.Email(),
                EmailConfirmed: false,
                PhoneNumber: string.Empty,
                SecurityStamp: f.Random.AlphaNumeric(32),
                UserDomainId: Guid.CreateVersion7(),
                UserDomain: userDomain,
                Roles: [.. f.Make(2, () => f.Name.JobTitle())],
                Claims: [.. f.Make(2, () => new ClaimDto(
                    Type: f.Random.Word(),
                    Value: f.Random.Word()
                ))]
            ))
            .Generate();

        Mock<IOptions<AccessTokenSettings>> accessTokenSettingsMock = new();
        Mock<IOptions<RefreshTokenSettings>> refreshTokenSettingsMock = new();

        accessTokenSettingsMock.Setup(x => x.Value).Returns(_accessTokenSettings);
        refreshTokenSettingsMock.Setup(x => x.Value).Returns(_refreshTokenSettings);

        _tokenService = new JwtTokenService(
            accessTokenSettingsMock.Object,
            refreshTokenSettingsMock.Object);
    }

    #endregion

    #region GenerateToken Tests

    [Trait("Category", "Unit")]
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
        jsonToken.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
        jsonToken.Issuer.Should().Be(_accessTokenSettings.Issuer);
        jsonToken.Audiences.Should().Contain(_accessTokenSettings.Audience);
        
        jsonToken.ValidTo.Should().BeCloseTo(
            DateTime.UtcNow.AddMinutes(_accessTokenSettings.ExpirationInMinutes), 
            TimeSpan.FromSeconds(5));
        
        jsonToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == _userDetailsTest.Id.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == _userDetailsTest.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == _userDetailsTest.UserName);
        jsonToken.Claims.Should().Contain(c => c.Type == "securityStamp" && c.Value == _userDetailsTest.SecurityStamp);
        jsonToken.Claims.Should().Contain(c => c.Type == "userDomainId" && c.Value == _userDetailsTest.UserDomainId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == "companyId" && c.Value == _userDetailsTest.UserDomain.CompanyId.ToString());
        
        foreach (var role in _userDetailsTest.Roles)
        {
            jsonToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == role);
        }
        
        foreach (var claim in _userDetailsTest.Claims)
        {
            jsonToken.Claims.Should().Contain(c => c.Type == claim.Type && c.Value == claim.Value);
        }
    }

    [Trait("Category", "Unit")]
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

    [Trait("Category", "Unit")]
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
   
        // Base claims: NameIdentifier, Email, Name, userDomainId, securityStamp, companyId + standard JWT claims (iat, nbf, exp, iss, aud)
        var baseClaims = 6; // Custom base claims
        var expectedClaims = baseClaims + userWithoutClaims.Roles.Count;
        
        var customClaims = jsonToken.Claims.Where(c => 
            c.Type != "nbf" && c.Type != "exp" && c.Type != "iat" && 
            c.Type != "iss" && c.Type != "aud");
        customClaims.Count().Should().Be(expectedClaims);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Generating access token should include correct number of claims")]
    public void GenerateToken_WhenCalled_ShouldIncludeCorrectNumberOfClaims()
    {
        // Act
        var token = _tokenService.GenerateToken(_userDetailsTest);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        // Base claims: NameIdentifier, Email, Name, userDomainId, securityStamp, companyId
        var baseClaims = 6;
        var totalExpected = baseClaims + _userDetailsTest.Roles.Count + _userDetailsTest.Claims.Count;
        
        var customClaims = jsonToken.Claims.Where(c => 
            c.Type != "nbf" && c.Type != "exp" && c.Type != "iat" && 
            c.Type != "iss" && c.Type != "aud");
        customClaims.Count().Should().Be(totalExpected);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Generating access token should have correct expiration time")]
    public void GenerateToken_WhenCalled_ShouldHaveCorrectExpirationTime()
    {
        // Arrange
        var beforeGeneration = DateTime.UtcNow;
        
        // Act
        var token = _tokenService.GenerateToken(_userDetailsTest);
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        var expectedExpiration = beforeGeneration.AddMinutes(_accessTokenSettings.ExpirationInMinutes);
        jsonToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region GenerateRefreshToken Tests

    [Trait("Category", "Unit")]
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
        jsonToken.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
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

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Generating refresh token should have correct expiration time")]
    public void GenerateRefreshToken_WhenCalled_ShouldHaveCorrectExpirationTime()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var securityStamp = _faker.Random.AlphaNumeric(32);
        var beforeGeneration = DateTime.UtcNow;
        
        // Act
        var token = _tokenService.GenerateRefreshToken(userId, securityStamp);
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        var expectedExpiration = beforeGeneration.AddDays(_refreshTokenSettings.ExpirationInDays);
        jsonToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(5));
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Generating refresh token should use different settings than access token")]
    public void GenerateRefreshToken_WhenCalled_ShouldUseDifferentSettingsThanAccessToken()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var securityStamp = _faker.Random.AlphaNumeric(32);
        
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken(userId, securityStamp);
        var accessToken = _tokenService.GenerateToken(_userDetailsTest);
        
        // Assert
        var handler = new JwtSecurityTokenHandler();
        var refreshJwt = handler.ReadJwtToken(refreshToken);
        var accessJwt = handler.ReadJwtToken(accessToken);
        
        refreshJwt.Issuer.Should().Be(_refreshTokenSettings.Issuer);
        accessJwt.Issuer.Should().Be(_accessTokenSettings.Issuer);
        
        refreshJwt.Audiences.Should().Contain(_refreshTokenSettings.Audience);
        accessJwt.Audiences.Should().Contain(_accessTokenSettings.Audience);
        
        // Different expiration times
        var refreshExpectedExpiration = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationInDays);
        var accessExpectedExpiration = DateTime.UtcNow.AddMinutes(_accessTokenSettings.ExpirationInMinutes);
        
        refreshJwt.ValidTo.Should().BeCloseTo(refreshExpectedExpiration, TimeSpan.FromSeconds(5));
        accessJwt.ValidTo.Should().BeCloseTo(accessExpectedExpiration, TimeSpan.FromSeconds(5));
    }
    #endregion
}