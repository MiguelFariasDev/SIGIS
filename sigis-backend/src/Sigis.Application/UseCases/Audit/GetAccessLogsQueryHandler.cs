using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Processa <see cref="GetAccessLogsQuery"/>, delegando a filtragem ao repositório de auditoria.</summary>
public sealed class GetAccessLogsQueryHandler
    : IRequestHandler<GetAccessLogsQuery, Result<IReadOnlyList<AccessLogResponse>>>
{
    private readonly IAccessLogRepository _accessLogRepository;

    /// <summary>Cria o handler de <see cref="GetAccessLogsQuery"/>.</summary>
    /// <param name="accessLogRepository">Repositório de registros de auditoria de acesso.</param>
    public GetAccessLogsQueryHandler(IAccessLogRepository accessLogRepository)
    {
        _accessLogRepository = accessLogRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AccessLogResponse>>> Handle(
        GetAccessLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _accessLogRepository.SearchAsync(
            request.PersonId, request.ProfessionalId, request.From, request.To, cancellationToken);

        var response = logs
            .Select(l => new AccessLogResponse(
                l.Id, l.PersonId, l.ProfessionalId, l.Action, l.LegalBasis, l.Justification, l.DateTime,
                l.IsCrossUnit()))
            .ToList();

        return Result<IReadOnlyList<AccessLogResponse>>.Success(response);
    }
}
