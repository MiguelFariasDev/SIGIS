namespace Sigis.Api.Contracts.Attendances;

/// <summary>Requisição de registro de comparecimento em um atendimento agendado.</summary>
/// <param name="Comparecimento">Situação de comparecimento: "COMPARECEU" ou "FALTOU".</param>
/// <param name="MainComplaint">Queixa principal relatada, opcional.</param>
public sealed record RegisterAttendanceComparecimentoRequest(string Comparecimento, string? MainComplaint);
