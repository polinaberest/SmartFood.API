namespace SmartFood.API.Contracts.Auth.Requests;

public record RegisterCompanyRequest : RegisterRequest
{
    public string OrganizationName { get; init; } = string.Empty;
    public string? Description { get; init; }
}