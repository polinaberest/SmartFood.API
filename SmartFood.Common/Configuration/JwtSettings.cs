namespace SmartFood.Common.Configuration;

public class JwtSettings
{
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int TokenValidityInMinutes { get; set; } = 5;
    public int RefreshTokenValidityInDays { get; set; } = 7;
}
