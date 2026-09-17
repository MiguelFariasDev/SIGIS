using FluentValidation;

namespace Sigis.Application.UseCases.SchoolHistories;

/// <summary>Validador de <see cref="AddSchoolHistoryCommand"/>: nome da escola, série e ano letivo são obrigatórios.</summary>
public sealed class AddSchoolHistoryCommandValidator : AbstractValidator<AddSchoolHistoryCommand>
{
    /// <summary>Cria o validador de <see cref="AddSchoolHistoryCommand"/>.</summary>
    public AddSchoolHistoryCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty().WithMessage("Identificador da pessoa é obrigatório.");
        RuleFor(x => x.SchoolName).NotEmpty().WithMessage("Nome da escola é obrigatório.");
        RuleFor(x => x.Grade).NotEmpty().WithMessage("Série é obrigatória.");
        RuleFor(x => x.SchoolYear).GreaterThan(0).WithMessage("Ano letivo é obrigatório.");
    }
}
