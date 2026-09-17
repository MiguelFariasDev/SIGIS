using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Audit;

/// <summary>
/// Consulta os registros de auditoria de acesso (RF13), filtrados
/// opcionalmente por pessoa, profissional e período.
/// </summary>
/// <param name="PersonId">Identificador da pessoa, opcional.</param>
/// <param name="ProfessionalId">Identificador do profissional, opcional.</param>
/// <param name="From">Início do período (UTC, inclusive), opcional.</param>
/// <param name="To">Fim do período (UTC, inclusive), opcional.</param>
public sealed record GetAccessLogsQuery(
    Guid? PersonId, Guid? ProfessionalId, DateTime? From, DateTime? To)
    : IRequest<Result<IReadOnlyList<AccessLogResponse>>>;
