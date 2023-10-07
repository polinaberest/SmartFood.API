using Microsoft.IdentityModel.Tokens;
using SmartFood.Common.Configuration;
using SmartFood.Infrastructure.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartFood.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings jwtSettings;
    private readonly SymmetricSecurityKey signingKey;
    private readonly SigningCredentials signingCredentials;

    public JwtTokenService(JwtSettings jwtSettings)
    {
        this.jwtSettings = jwtSettings;

        signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    public JwtSecurityToken CreateToken(IEnumerable<Claim> authClaims)
    {
        var token = new JwtSecurityToken(
            issuer: jwtSettings.ValidIssuer,
            audience: jwtSettings.ValidAudience,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.TokenValidityInMinutes),
            claims: authClaims,
            signingCredentials: signingCredentials
        );

        return token;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}