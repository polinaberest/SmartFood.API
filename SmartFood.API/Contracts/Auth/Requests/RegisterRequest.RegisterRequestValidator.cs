using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartFood.Common.Constants;
using SmartFood.Domain;

namespace SmartFood.API.Contracts.Auth.Requests;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private static readonly IEnumerable<string> RolesAllowedForRegistration =
        UserRoles.AvailableRoles.Except(new[] { UserRoles.Administrator }).ToArray();

    public RegisterRequestValidator(ApplicationDbContext dbContext)
    {
        // TODO: Define validation rules for each property

        RuleFor(c => c.Password).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MinimumLength(3);
        RuleFor(c => c.Role)
            .Must(c => RolesAllowedForRegistration.Contains(c.ToLower())).WithMessage("Registration role is invalid.");

        RuleFor(c => c.Email)
            .EmailAddress()
            .MustAsync((e, _) => dbContext.Users.AllAsync(u => u.Email != e)).WithMessage("User with this email already exists.");
    }
}