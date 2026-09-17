using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Exporta os registros de auditoria de acesso, filtrados como em <see cref="GetAccessLogsQuery"/>, em CSV.</summary>
public sealed record ExportAccessLogsCsvQuery(
    Guid? PersonId, Guid? ProfessionalId, DateTime? From, DateTime? To) : IRequest<Result<string>>;
