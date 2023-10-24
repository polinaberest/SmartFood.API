using FluentValidation;
using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class OrderValidator : AbstractValidator<Order>
{
    private static readonly RequestStatus[] StatusesAllowed = Enum.GetValues(typeof(RequestStatus)) as RequestStatus[];

    public OrderValidator(ApplicationDbContext dbContext)
    {
        RuleFor(r => r.Status)
            .Must(r => StatusesAllowed.Contains(r)).WithMessage("Status is invalid. Allowed values are: " + string.Join(", ", StatusesAllowed));
        RuleFor(r => r.Count).GreaterThan(0);
        RuleFor(r => r.TotalPrice).GreaterThan(0);
    }
}