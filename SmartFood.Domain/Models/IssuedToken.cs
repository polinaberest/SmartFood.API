namespace SmartFood.Domain.Models;

public class IssuedToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }

    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpirationTime { get; set; } = DateTime.UtcNow;
}