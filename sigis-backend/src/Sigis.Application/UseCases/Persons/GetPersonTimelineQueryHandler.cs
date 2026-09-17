using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Persons;

/// <summary>
/// Processa a consulta da linha do tempo consolidada de uma pessoa (RF08),
/// agregando atendimentos e encaminhamentos de todas as unidades da rede, e
/// registrando auditoria de acesso (RF13) sempre que a consulta cruza a
/// fronteira de secretarias (LGPD, art. 11, II).
/// </summary>
/// <remarks>
/// Escopo desta versão: histórico de fila (<c>QueueEntry</c>) não é incluído
/// na linha do tempo — apenas <c>Attendance</c> e <c>Referral</c>, que já
/// expõem <c>GetByPersonAsync</c>. Ampliar para incluir filas exigiria um
/// método equivalente em <c>IQueueEntryRepository</c>, fora do escopo deste
/// ajuste.
/// </remarks>
public sealed class GetPersonTimelineQueryHandler : IRequestHandler<GetPersonTimelineQuery, Result<TimelineResponse>>
{
    private const string CompleteLevel = "completo";
    private const string MetadataLevel = "metadados";

    private readonly IPersonRepository _personRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IReferralRepository _referralRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de consulta da linha do tempo.</summary>
    public GetPersonTimelineQueryHandler(
        IPersonRepository personRepository,
        IAttendanceRepository attendanceRepository,
        IReferralRepository referralRepository,
        IServiceUnitRepository serviceUnitRepository,
        IAccessLogRepository accessLogRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _attendanceRepository = attendanceRepository;
        _referralRepository = referralRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _accessLogRepository = accessLogRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<TimelineResponse>> Handle(GetPersonTimelineQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<TimelineResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var level = string.Equals(request.Nivel, MetadataLevel, StringComparison.OrdinalIgnoreCase)
            ? MetadataLevel
            : CompleteLevel;

        var attendances = await _attendanceRepository.GetByPersonAsync(request.PersonId, cancellationToken);
        var referrals = await _referralRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var unitIds = attendances.Select(a => a.UnitId)
            .Concat(referrals.Select(r => r.OriginUnitId))
            .Concat(referrals.Select(r => r.DestinationUnitId))
            .Distinct()
            .ToList();

        var units = new Dictionary<Guid, ServiceUnit>();
        foreach (var unitId in unitIds)
        {
            var unit = await _serviceUnitRepository.GetByIdAsync(unitId, cancellationToken);
            if (unit is not null)
                units[unitId] = unit;
        }

        ServiceUnit? requesterUnit = _currentUserService.UnitId is Guid requesterUnitId
            ? await _serviceUnitRepository.GetByIdAsync(requesterUnitId, cancellationToken)
            : null;

        var isCrossSecretariat = requesterUnit is not null && units.Values.Any(u => u.Secretariat != requesterUnit.Secretariat);

        var requiresJustification = level == CompleteLevel || isCrossSecretariat;
        if (requiresJustification && string.IsNullOrWhiteSpace(request.Justificativa))
            return Result<TimelineResponse>.Failure(AuthErrors.Forbidden);

        var includeDetails = level == CompleteLevel;

        var entries = attendances
            .Select(a => new TimelineEntry(
                a.DateTime,
                "Atendimento",
                units.TryGetValue(a.UnitId, out var unit) ? unit.Name : "Unidade desconhecida",
                includeDetails ? (a.MainComplaint ?? a.FormData) : null))
            .Concat(referrals.Select(r => new TimelineEntry(
                r.ReferralDate,
                "Encaminhamento",
                units.TryGetValue(r.DestinationUnitId, out var destUnit) ? destUnit.Name : "Unidade desconhecida",
                includeDetails ? r.Reason : null)))
            .OrderByDescending(e => e.DateTime)
            .ToList();

        if (isCrossSecretariat && _currentUserService.ProfessionalId is Guid professionalId)
        {
            var accessLogResult = AccessLog.Create(
                request.PersonId,
                professionalId,
                action: "VisualizacaoTimeline",
                // Coluna access_log.legal_basis é varchar(50) — texto precisa caber.
                legalBasis: "LGPD art. 11, II — políticas públicas",
                dateTime: _dateTimeProvider.UtcNow,
                isCrossUnit: true,
                justification: request.Justificativa);

            if (accessLogResult.IsSuccess)
            {
                await _accessLogRepository.AddAsync(accessLogResult.Value, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return Result<TimelineResponse>.Success(new TimelineResponse(request.PersonId, level, entries));
    }
}
