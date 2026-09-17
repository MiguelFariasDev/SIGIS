using FluentValidation;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Validador de <see cref="GrantConsentCommand"/>: a versão do termo é obrigatória.</summary>
public sealed class GrantConsentCommandValidator : AbstractValidator<GrantConsentCommand>
{
    /// <summary>Cria o validador de <see cref="GrantConsentCommand"/>.</summary>
    public GrantConsentCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty().WithMessage("Identificador da pessoa é obrigatório.");
        RuleFor(x => x.Version).NotEmpty().WithMessage("Versão do termo de consentimento é obrigatória.");
    }
}
