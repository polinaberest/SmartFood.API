using FluentValidation;
using SmartFood.Domain.Models.Interfaces;

namespace SmartFood.Domain.Models;

public class FridgeValidator : AbstractValidator<Fridge>
{
    public FridgeValidator(ApplicationDbContext dbContext)
    {
        RuleFor(f => f.PlacementDescription).NotEmpty();
        RuleFor(f => f.URI).NotEmpty();
    }
}