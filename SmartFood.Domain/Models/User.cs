using Microsoft.AspNetCore.Identity;

namespace SmartFood.Domain.Models;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    public List<IssuedToken> IssuedTokens { get; set; } = new();
}