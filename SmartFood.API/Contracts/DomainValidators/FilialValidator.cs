using FluentValidation;

namespace SmartFood.Domain.Models;

public class FilialValidator : AbstractValidator<Filial>
{
    public FilialValidator(ApplicationDbContext dbContext)
    {
        RuleFor(c => c.Name).NotEmpty().MinimumLength(3);
        RuleFor(c => c.Address).NotEmpty().MinimumLength(3);
    }
}