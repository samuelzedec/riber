using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Riber.Application.Abstractions.Services;
using Riber.Application.Models;
using Riber.Infrastructure.Settings;

namespace Riber.Infrastructure.Services.Authentication;

public sealed class JwtTokenService(
    IOptions<AccessTokenSettings> accessTokenSettings,
    IOptions<RefreshTokenSettings> refreshTokenSettings)
    : ITokenService
{
    public string GenerateToken(UserDetailsModel user)
    {
        var companyId = user.UserDomain.CompanyId.ToString() ?? string.Empty;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
            new("userDomainId", user.UserDomainId.ToString()),
            new("securityStamp", user.SecurityStamp),
            new("companyId", companyId)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Claims.Select(claim => new Claim(claim.Type, claim.Value)));

        return CreateJwtToken(
            claims,
            DateTime.UtcNow.AddMinutes(accessTokenSettings.Value.ExpirationInMinutes),
            accessTokenSettings.Value.SecretKey,
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
            refreshTokenSettings.Value.SecretKey,
            refreshTokenSettings.Value.Issuer,
            refreshTokenSettings.Value.Audience
        );
    }

    #region Helpers

    private static string CreateJwtToken(
        IEnumerable<Claim> claims, 
        DateTime expires, 
        string secretKey,
        string issuer, 
        string audience)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    #endregion
}