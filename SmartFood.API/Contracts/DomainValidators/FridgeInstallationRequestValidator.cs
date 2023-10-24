using FluentValidation;

namespace SmartFood.Domain.Models;

public class FridgeInstallationRequestValidator : AbstractValidator<FridgeInstallationRequest>
{
    private static readonly RequestStatus[] StatusesAllowed = Enum.GetValues(typeof(RequestStatus)) as RequestStatus[];

    public FridgeInstallationRequestValidator(ApplicationDbContext dbContext)
    {
        RuleFor(r => r.Status)
            .Must(r => StatusesAllowed.Contains(r)).WithMessage("Status is invalid. Allowed values are: " + string.Join(", ", StatusesAllowed));
        RuleFor(r => r.PlacementDescription).NotEmpty();
    }
}

