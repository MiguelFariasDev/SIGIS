namespace Sigis.Application.UseCases.Attendances;

/// <summary>Resumo de um atendimento registrado, para exibição.</summary>
/// <param name="Id">Identificador do atendimento.</param>
/// <param name="PersonId">Identificador da pessoa atendida.</param>
/// <param name="UnitId">Identificador da unidade de serviço.</param>
/// <param name="ProfessionalId">Identificador do profissional responsável.</param>
/// <param name="DateTime">Data e hora (UTC) do atendimento.</param>
/// <param name="SessionType">Tipo de sessão/anamnese.</param>
/// <param name="SessionNumber">Número sequencial da sessão, para o mesmo tipo e pessoa.</param>
/// <param name="Status">Situação de comparecimento.</param>
/// <param name="FormData">Dados do formulário específico, em JSON, quando informados.</param>
/// <param name="TriagedByProfessionalId">Identificador do profissional que triou, quando informado.</param>
/// <param name="MainComplaint">Queixa principal relatada na triagem, quando informada.</param>
/// <param name="CreatedAt">Data e hora (UTC) de criação do registro.</param>
public sealed record AttendanceResponse(
    Guid Id,
    Guid PersonId,
    Guid UnitId,
    Guid ProfessionalId,
    DateTime DateTime,
    string SessionType,
    int SessionNumber,
    string Status,
    string? FormData,
    Guid? TriagedByProfessionalId,
    string? MainComplaint,
    DateTime CreatedAt);
