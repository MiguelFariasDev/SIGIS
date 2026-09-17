namespace Sigis.Domain.Enums;

/// <summary>
/// Tipo de consentimento LGPD concedido por uma pessoa (ou seu responsável)
/// para tratamento de dados por uma finalidade específica (LGPD, art. 8º e
/// art. 11, I).
/// </summary>
public enum ConsentType
{
    /// <summary>Consentimento para tratamento clínico/de saúde.</summary>
    Clinical = 1,

    /// <summary>Consentimento para tratamento educacional.</summary>
    Educational = 2,

    /// <summary>Consentimento para tratamento pela assistência social.</summary>
    SocialAssistance = 3,

    /// <summary>Consentimento para uso em pesquisa.</summary>
    Research = 4
}
