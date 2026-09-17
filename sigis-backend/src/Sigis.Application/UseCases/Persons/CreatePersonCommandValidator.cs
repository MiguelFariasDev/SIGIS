using FluentValidation;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Validador de forma de <see cref="CreatePersonCommand"/> — regras de negócio ficam nos value objects e na entidade.</summary>
public sealed class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    /// <summary>Cria o validador de <see cref="CreatePersonCommand"/>.</summary>
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Nome completo é obrigatório.");
        RuleFor(x => x.BirthDate).NotEqual(default(DateOnly)).WithMessage("Data de nascimento é obrigatória.");
    }
}
