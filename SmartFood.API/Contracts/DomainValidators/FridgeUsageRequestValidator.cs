using FluentValidation;
using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class FridgeUsageRequestValidator : AbstractValidator<FridgeUsageRequest>
{
    private static readonly RequestStatus[] StatusesAllowed = Enum.GetValues(typeof(RequestStatus)) as RequestStatus[];

    public FridgeUsageRequestValidator(ApplicationDbContext dbContext)
    {
        RuleFor(r => r.Status)
            .Must(r => StatusesAllowed.Contains(r)).WithMessage("Status is invalid. Allowed values are: " + string.Join(", ", StatusesAllowed));
    }
}