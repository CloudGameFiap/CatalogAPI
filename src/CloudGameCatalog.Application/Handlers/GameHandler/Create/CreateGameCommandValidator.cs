using FluentValidation;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Create;

public sealed class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve conter no máximo 100 caracteres.");
    }
}
