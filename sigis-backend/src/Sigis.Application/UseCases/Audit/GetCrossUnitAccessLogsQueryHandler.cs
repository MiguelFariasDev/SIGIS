using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Processa <see cref="GetCrossUnitAccessLogsQuery"/>, delegando ao repositório de auditoria.</summary>
public sealed class GetCrossUnitAccessLogsQueryHandler
    : IRequestHandler<GetCrossUnitAccessLogsQuery, Result<IReadOnlyList<CrossAccessResponse>>>
{
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IProfessionalRepository _professionalRepository;

    /// <summary>Cria o handler de <see cref="GetCrossUnitAccessLogsQuery"/>.</summary>
    /// <param name="accessLogRepository">Repositório de registros de auditoria de acesso.</param>
    /// <param name="personRepository">Repositório de pessoas, para resolver o nome exibido.</param>
    /// <param name="professionalRepository">Repositório de profissionais, para resolver o nome exibido.</param>
    public GetCrossUnitAccessLogsQueryHandler(
        IAccessLogRepository accessLogRepository,
        IPersonRepository personRepository,
        IProfessionalRepository professionalRepository)
    {
        _accessLogRepository = accessLogRepository;
        _personRepository = personRepository;
        _professionalRepository = professionalRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<CrossAccessResponse>>> Handle(
        GetCrossUnitAccessLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _accessLogRepository.GetCrossUnitAsync(cancellationToken);

        var response = new List<CrossAccessResponse>(logs.Count);
        foreach (var log in logs)
            response.Add(await AccessLogResponseMapper.MapCrossAsync(log, _personRepository, _professionalRepository, cancellationToken));

        return Result<IReadOnlyList<CrossAccessResponse>>.Success(response);
    }
}
