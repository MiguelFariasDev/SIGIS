namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Consentimentos LGPD (<c>PersonConsent</c>).</summary>
public static class PersonConsentErrors
{
    /// <summary>Já existe consentimento ativo deste tipo.</summary>
    public static readonly Error ConsentimentoAtivoJaExiste = new("CONSENT_001", "Já existe consentimento ativo deste tipo.", ErrorType.Conflict);

    /// <summary>Data de revogação não pode ser anterior à concessão.</summary>
    public static readonly Error RevogacaoAnteriorAConcessao = new("CONSENT_002", "Data de revogação não pode ser anterior à concessão.");

    /// <summary>Consentimento clínico é obrigatório para cadastro.</summary>
    public static readonly Error ConsentimentoClinicoObrigatorio = new("CONSENT_003", "Consentimento clínico é obrigatório para cadastro.");

    /// <summary>Consentimento já revogado não pode ser reativado.</summary>
    public static readonly Error RevogadoNaoPodeReativar = new("CONSENT_004", "Consentimento já revogado não pode ser reativado — crie um novo registro.", ErrorType.Conflict);
}
