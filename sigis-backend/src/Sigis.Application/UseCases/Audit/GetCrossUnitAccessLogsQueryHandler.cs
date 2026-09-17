using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Processa <see cref="GetCrossUnitAccessLogsQuery"/>, delegando ao repositório de auditoria.</summary>
public sealed class GetCrossUnitAccessLogsQueryHandler
    : IRequestHandler<GetCrossUnitAccessLogsQuery, Result<IReadOnlyList<CrossAccessResponse>>>
{
    private readonly IAccessLogRepository _accessLogRepository;

    /// <summary>Cria o handler de <see cref="GetCrossUnitAccessLogsQuery"/>.</summary>
    /// <param name="accessLogRepository">Repositório de registros de auditoria de acesso.</param>
    public GetCrossUnitAccessLogsQueryHandler(IAccessLogRepository accessLogRepository)
    {
        _accessLogRepository = accessLogRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<CrossAccessResponse>>> Handle(
        GetCrossUnitAccessLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _accessLogRepository.GetCrossUnitAsync(cancellationToken);

        var response = logs
            .Select(l => new CrossAccessResponse(
                l.Id, l.PersonId, l.ProfessionalId, l.Action, l.LegalBasis, l.Justification, l.DateTime))
            .ToList();

        return Result<IReadOnlyList<CrossAccessResponse>>.Success(response);
    }
}
