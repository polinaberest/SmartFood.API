using FluentValidation;

namespace SmartFood.API.Contracts.Auth.Requests;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(c => c.Token).NotEmpty();
        RuleFor(c => c.RefreshToken).NotEmpty();
    }
}