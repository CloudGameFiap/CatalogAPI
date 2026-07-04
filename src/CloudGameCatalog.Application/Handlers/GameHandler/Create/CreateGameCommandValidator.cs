using FluentValidation;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Create;

public sealed class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(80).WithMessage("O nome deve conter no máximo 80 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve conter no máximo 100 caracteres.");

        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("O genero é obrigatório.")
            .MaximumLength(50).WithMessage("O genero deve conter no máximo 50 caracteres.");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty()
            .WithMessage("A data de lançamento é obrigatoria.")
            .GreaterThan(DateTime.Now).WithMessage("Data de lançamento inválida.");
            
    }
}
