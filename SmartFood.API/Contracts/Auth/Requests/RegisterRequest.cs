namespace SmartFood.API.Contracts.Auth.Requests;

public record RegisterRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string OrganizationName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Role { get; init; } = string.Empty;
}