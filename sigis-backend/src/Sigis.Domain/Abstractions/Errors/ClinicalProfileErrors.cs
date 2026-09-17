namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Perfil Clínico NASF (<c>PersonClinicalProfile</c>).</summary>
public static class ClinicalProfileErrors
{
    /// <summary>Perfil clínico já cadastrado para esta pessoa.</summary>
    public static readonly Error JaCadastrado = new("CLINICAL_001", "Perfil clínico já cadastrado para esta pessoa.");

    /// <summary>Número do prontuário já existe nesta unidade.</summary>
    public static readonly Error ProntuarioDuplicado = new("CLINICAL_002", "Número do prontuário já existe nesta unidade.");

    /// <summary>Hipótese diagnóstica é obrigatória para pacientes NASF.</summary>
    public static readonly Error HipoteseDiagnosticaObrigatoria = new("CLINICAL_003", "Hipótese diagnóstica é obrigatória para pacientes NASF.");

    /// <summary>APS de referência deve ser da Secretaria de Saúde.</summary>
    public static readonly Error ApsDeveSerSaude = new("CLINICAL_004", "APS de referência deve ser uma unidade da Secretaria de Saúde.");
}
