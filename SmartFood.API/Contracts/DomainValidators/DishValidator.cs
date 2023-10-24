using FluentValidation;

namespace SmartFood.Domain.Models;

public class DishValidator : AbstractValidator<Dish>
{
    public DishValidator (ApplicationDbContext dbContext)
    {
        RuleFor(d => d.Name).NotEmpty().MinimumLength(3);
        RuleFor(d => d.Price).GreaterThan(0);
    }
}