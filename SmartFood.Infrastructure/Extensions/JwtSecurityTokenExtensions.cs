using System.IdentityModel.Tokens.Jwt;

namespace SmartFood.Infrastructure.Extensions;

public static class JwtSecurityTokenExtensions
{
    public static string SerializeToString(this JwtSecurityToken token) => new JwtSecurityTokenHandler().WriteToken(token);
}