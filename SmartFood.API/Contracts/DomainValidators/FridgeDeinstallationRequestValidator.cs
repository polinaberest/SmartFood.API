using FluentValidation;
using SmartFood.Common.Constants;
using System.Linq;

namespace SmartFood.Domain.Models;

public class FridgeDeinstallationRequestValidator : AbstractValidator<FridgeDeinstallationRequest>
{
    private static readonly RequestStatus[] StatusesAllowed = Enum.GetValues(typeof(RequestStatus)) as RequestStatus[];

    public FridgeDeinstallationRequestValidator(ApplicationDbContext dbContext)
    {
        RuleFor(r => r.Status)
            .Must(r => StatusesAllowed.Contains(r)).WithMessage("Status is invalid. Allowed values are: " + string.Join(", ", StatusesAllowed));
    }
}