using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Processa <see cref="GetAccessLogsQuery"/>, delegando a filtragem ao repositório de auditoria.</summary>
public sealed class GetAccessLogsQueryHandler
    : IRequestHandler<GetAccessLogsQuery, Result<IReadOnlyList<AccessLogResponse>>>
{
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IProfessionalRepository _professionalRepository;

    /// <summary>Cria o handler de <see cref="GetAccessLogsQuery"/>.</summary>
    /// <param name="accessLogRepository">Repositório de registros de auditoria de acesso.</param>
    /// <param name="personRepository">Repositório de pessoas, para resolver o nome exibido.</param>
    /// <param name="professionalRepository">Repositório de profissionais, para resolver o nome exibido.</param>
    public GetAccessLogsQueryHandler(
        IAccessLogRepository accessLogRepository,
        IPersonRepository personRepository,
        IProfessionalRepository professionalRepository)
    {
        _accessLogRepository = accessLogRepository;
        _personRepository = personRepository;
        _professionalRepository = professionalRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AccessLogResponse>>> Handle(
        GetAccessLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _accessLogRepository.SearchAsync(
            request.PersonId, request.ProfessionalId, request.From, request.To, cancellationToken);

        var response = new List<AccessLogResponse>(logs.Count);
        foreach (var log in logs)
            response.Add(await AccessLogResponseMapper.MapAsync(log, _personRepository, _professionalRepository, cancellationToken));

        return Result<IReadOnlyList<AccessLogResponse>>.Success(response);
    }
}
