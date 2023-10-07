namespace SmartFood.Infrastructure.Commands.Auth;

public record RegisterCommand
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string OrganizationName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Role { get; init; } = string.Empty;
}