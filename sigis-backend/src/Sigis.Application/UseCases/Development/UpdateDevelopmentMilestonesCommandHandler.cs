using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Development;

/// <summary>
/// Processa a criação (se ainda não existir) ou atualização dos marcos de
/// desenvolvimento de uma pessoa (get-or-create — relação 1:1).
/// </summary>
public sealed class UpdateDevelopmentMilestonesCommandHandler
    : IRequestHandler<UpdateDevelopmentMilestonesCommand, Result<DevelopmentMilestonesResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IDevelopmentMilestonesRepository _developmentMilestonesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de atualização de marcos de desenvolvimento.</summary>
    public UpdateDevelopmentMilestonesCommandHandler(
        IPersonRepository personRepository,
        IDevelopmentMilestonesRepository developmentMilestonesRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _developmentMilestonesRepository = developmentMilestonesRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<DevelopmentMilestonesResponse>> Handle(
        UpdateDevelopmentMilestonesCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<DevelopmentMilestonesResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var existing = await _developmentMilestonesRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        if (existing is null)
        {
            var createResult = Sigis.Domain.Entities.DevelopmentMilestones.Create(
                request.PersonId, _dateTimeProvider.UtcNow, request.AgeWalkedMonths, request.AgeTalkedMonths,
                request.LocomotionDifficulty, request.CoordinationDifficulty, request.VisualDifficulty,
                request.HearingDifficulty, request.SpeechProblems, request.CommandComprehension,
                request.CommunicationForm, request.ManualDominance);

            if (createResult.IsFailure)
                return Result<DevelopmentMilestonesResponse>.Failure(createResult.Error);

            await _developmentMilestonesRepository.AddAsync(createResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<DevelopmentMilestonesResponse>.Success(DevelopmentMilestonesMapper.ToResponse(createResult.Value));
        }

        var updateResult = existing.UpdateData(
            request.AgeWalkedMonths, request.AgeTalkedMonths, request.LocomotionDifficulty,
            request.CoordinationDifficulty, request.VisualDifficulty, request.HearingDifficulty,
            request.SpeechProblems, request.CommandComprehension, request.CommunicationForm, request.ManualDominance);

        if (updateResult.IsFailure)
            return Result<DevelopmentMilestonesResponse>.Failure(updateResult.Error);

        await _developmentMilestonesRepository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DevelopmentMilestonesResponse>.Success(DevelopmentMilestonesMapper.ToResponse(existing));
    }
}
