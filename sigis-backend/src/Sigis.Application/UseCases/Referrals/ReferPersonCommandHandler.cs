using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>
/// Processa a criação de um encaminhamento de pessoa entre unidades da rede.
/// Apenas um profissional da própria unidade de origem (ou um Coordenador,
/// com visão de rede completa) pode iniciar o encaminhamento.
/// </summary>
public sealed class ReferPersonCommandHandler : IRequestHandler<ReferPersonCommand, Result<ReferralResponse>>
{
    private readonly IReferralRepository _referralRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de criação de encaminhamento.</summary>
    public ReferPersonCommandHandler(
        IReferralRepository referralRepository,
        IPersonRepository personRepository,
        IServiceUnitRepository serviceUnitRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _referralRepository = referralRepository;
        _personRepository = personRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ReferralResponse>> Handle(ReferPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<ReferralResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var originUnit = await _serviceUnitRepository.GetByIdAsync(request.OriginUnitId, cancellationToken);
        if (originUnit is null)
            return Result<ReferralResponse>.Failure(CommonErrors.NotFound);

        var destinationUnit = await _serviceUnitRepository.GetByIdAsync(request.DestinationUnitId, cancellationToken);
        if (destinationUnit is null)
            return Result<ReferralResponse>.Failure(CommonErrors.NotFound);

        // Só um profissional da própria unidade de origem — ou um
        // Coordenador, com visão de rede completa (RF12) — pode iniciar o
        // encaminhamento; evita que qualquer unidade encaminhe "em nome" de
        // outra sem estar de fato envolvida no caso.
        var isCoordinator = _currentUserService.Role == RbacRole.Coordinator;
        if (!isCoordinator && _currentUserService.UnitId != request.OriginUnitId)
            return Result<ReferralResponse>.Failure(AuthErrors.Forbidden);

        if (!Enum.TryParse<QueuePriority>(request.Priority, ignoreCase: true, out var priority))
            return Result<ReferralResponse>.Failure(new Error(
                "REFERRAL_VALIDATION", "Prioridade inválida (aceitos: Urgent, ShortTerm, WaitingList)."));

        var createResult = Referral.Create(
            request.PersonId, request.OriginUnitId, request.DestinationUnitId, request.Reason, priority,
            _dateTimeProvider.UtcNow);

        if (createResult.IsFailure)
            return Result<ReferralResponse>.Failure(createResult.Error);

        var referral = createResult.Value;
        await _referralRepository.AddAsync(referral, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ReferralResponse>.Success(
            await ReferralResponseMapper.MapAsync(referral, _serviceUnitRepository, cancellationToken));
    }
}
