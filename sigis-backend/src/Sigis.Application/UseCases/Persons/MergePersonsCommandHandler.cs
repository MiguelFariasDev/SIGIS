using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Persons;

/// <summary>
/// Processa a mesclagem de dois cadastros de pessoa duplicados: reatribui
/// atendimentos, encaminhamentos e entradas de fila do cadastro de origem
/// para o de destino, copia os responsáveis legais, e — quando referenciado
/// — marca o alerta de duplicidade como resolvido (mesclado).
/// </summary>
/// <remarks>
/// Escopo desta versão: o cadastro de origem não é excluído nem marcado
/// como mesclado (a entidade <c>Person</c> não possui esse campo hoje) —
/// permanece consultável, com os responsáveis legais duplicados em ambos os
/// cadastros. Os 7 módulos de cobertura (histórico escolar, composição
/// familiar, etc.) não são reatribuídos nesta versão — ficam para uma
/// evolução futura, dado que pertencem a outros módulos deste ajuste.
/// </remarks>
public sealed class MergePersonsCommandHandler : IRequestHandler<MergePersonsCommand, Result<MergeResult>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IReferralRepository _referralRepository;
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de mesclagem de pessoas.</summary>
    public MergePersonsCommandHandler(
        IPersonRepository personRepository,
        IDuplicateAlertRepository duplicateAlertRepository,
        IAttendanceRepository attendanceRepository,
        IReferralRepository referralRepository,
        IQueueEntryRepository queueEntryRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _duplicateAlertRepository = duplicateAlertRepository;
        _attendanceRepository = attendanceRepository;
        _referralRepository = referralRepository;
        _queueEntryRepository = queueEntryRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<MergeResult>> Handle(MergePersonsCommand request, CancellationToken cancellationToken)
    {
        if (request.SourcePersonId == request.TargetPersonId)
            return Result<MergeResult>.Failure(PersonErrors.MesclagemMesmaPessoa);

        var source = await _personRepository.GetByIdAsync(request.SourcePersonId, cancellationToken);
        if (source is null)
            return Result<MergeResult>.Failure(PersonErrors.PessoaNaoEncontrada);

        var target = await _personRepository.GetByIdAsync(request.TargetPersonId, cancellationToken);
        if (target is null)
            return Result<MergeResult>.Failure(PersonErrors.PessoaNaoEncontrada);

        if (request.DuplicateAlertId is Guid alertId)
        {
            var alert = await _duplicateAlertRepository.GetByIdAsync(alertId, cancellationToken);
            if (alert is null)
                return Result<MergeResult>.Failure(CommonErrors.NotFound);

            var professionalId = _currentUserService.ProfessionalId ?? Guid.Empty;
            var confirmResult = alert.ConfirmMerge(professionalId, _dateTimeProvider.UtcNow);
            if (confirmResult.IsFailure)
                return Result<MergeResult>.Failure(confirmResult.Error);

            await _duplicateAlertRepository.UpdateAsync(alert, cancellationToken);
        }

        var attendancesReassigned = await _attendanceRepository.ReassignPersonAsync(source.Id, target.Id, cancellationToken);
        var referralsReassigned = await _referralRepository.ReassignPersonAsync(source.Id, target.Id, cancellationToken);
        var queueEntriesReassigned = await _queueEntryRepository.ReassignPersonAsync(source.Id, target.Id, cancellationToken);

        var guardiansCopied = 0;
        foreach (var guardian in source.Guardians.ToList())
        {
            var addResult = target.AddGuardian(guardian.Name, guardian.Relationship, guardian.Cns, guardian.BirthDate);
            if (addResult.IsSuccess)
                guardiansCopied++;
        }

        await _personRepository.UpdateAsync(target, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<MergeResult>.Success(new MergeResult(
            target.Id, source.Id, attendancesReassigned, referralsReassigned, queueEntriesReassigned, guardiansCopied));
    }
}
