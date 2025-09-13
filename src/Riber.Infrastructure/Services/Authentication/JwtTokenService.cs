using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Riber.Application.Abstractions.Services;
using Riber.Application.DTOs;
using Riber.Infrastructure.Settings;

namespace Riber.Infrastructure.Services.Authentication;

public sealed class JwtTokenService(
    IOptions<AccessTokenSettings> accessTokenSettings,
    IOptions<RefreshTokenSettings> refreshTokenSettings,
    ICertificateService certificateService)
    : ITokenService
{
    public string GenerateToken(UserDetailsDTO user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
            new("userDomainId", user.UserDomainId.ToString()),
            new("securityStamp", user.SecurityStamp),
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Claims.Select(claim => new Claim(claim.Type, claim.Value)));

        return CreateJwtToken(
            claims,
            DateTime.UtcNow.AddMinutes(accessTokenSettings.Value.ExpirationInMinutes),
            accessTokenSettings.Value.Key,
            accessTokenSettings.Value.Password,
            accessTokenSettings.Value.Issuer,
            accessTokenSettings.Value.Audience
        );
    }

    public string GenerateRefreshToken(Guid userId, string securityStamp)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("tokenType", "refresh"),
            new("securityStamp", securityStamp),
        };

        return CreateJwtToken(
            claims,
            DateTime.UtcNow.AddDays(refreshTokenSettings.Value.ExpirationInDays),
            refreshTokenSettings.Value.Key,
            refreshTokenSettings.Value.Password,
            refreshTokenSettings.Value.Issuer,
            refreshTokenSettings.Value.Audience
        );
    }

    #region Helpers

    private string CreateJwtToken(
        IEnumerable<Claim> claims, 
        DateTime expires, 
        string key, 
        string password,
        string issuer, 
        string audience)
    {
        var certificate = certificateService.LoadCertificate(key, password);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new X509SecurityKey(certificate),
                SecurityAlgorithms.RsaSha256
            )
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    #endregion
}