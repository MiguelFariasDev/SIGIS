namespace Sigis.Application.UseCases.Audit;

/// <summary>Registro de auditoria de um acesso que cruzou a fronteira de unidades/secretarias (RNF02).</summary>
/// <param name="Id">Identificador do registro de auditoria.</param>
/// <param name="PersonId">Identificador da pessoa cujos dados foram acessados.</param>
/// <param name="PersonName">Nome da pessoa cujos dados foram acessados.</param>
/// <param name="ProfessionalId">Identificador do profissional que realizou o acesso.</param>
/// <param name="ProfessionalName">Nome do profissional que realizou o acesso.</param>
/// <param name="Action">Ação de auditoria realizada.</param>
/// <param name="LegalBasis">Base legal (LGPD, art. 11) que amparou o acesso.</param>
/// <param name="Justification">Justificativa do acesso entre unidades (obrigatória neste caso).</param>
/// <param name="DateTime">Data e hora (UTC) do acesso.</param>
public sealed record CrossAccessResponse(
    Guid Id,
    Guid PersonId,
    string PersonName,
    Guid ProfessionalId,
    string ProfessionalName,
    string Action,
    string LegalBasis,
    string? Justification,
    DateTime DateTime);
