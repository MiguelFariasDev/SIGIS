using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Registra um novo atendimento de uma pessoa em uma unidade de serviço.</summary>
/// <param name="PersonId">Identificador da pessoa atendida.</param>
/// <param name="UnitId">Identificador da unidade de serviço.</param>
/// <param name="ProfessionalId">Identificador do profissional responsável.</param>
/// <param name="DateTime">Data e hora (UTC) do atendimento.</param>
/// <param name="SessionType">Tipo de sessão/anamnese, como texto (nome do enum <see cref="Sigis.Domain.Enums.SessionType"/>).</param>
/// <param name="FormData">Dados do formulário específico, em JSON, opcional.</param>
/// <param name="TriagedByProfessionalId">Identificador do profissional que realizou a triagem, opcional.</param>
/// <param name="MainComplaint">Queixa principal relatada na triagem, opcional.</param>
public sealed record RegisterAttendanceCommand(
    Guid PersonId,
    Guid UnitId,
    Guid ProfessionalId,
    DateTime DateTime,
    string SessionType,
    string? FormData,
    Guid? TriagedByProfessionalId,
    string? MainComplaint) : IRequest<Result<AttendanceResponse>>;
