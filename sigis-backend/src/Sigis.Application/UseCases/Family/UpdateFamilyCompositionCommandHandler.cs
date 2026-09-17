using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Family;

/// <summary>
/// Processa a criação (se ainda não existir) ou atualização da composição
/// familiar de uma pessoa (get-or-create — relação 1:1).
/// </summary>
public sealed class UpdateFamilyCompositionCommandHandler
    : IRequestHandler<UpdateFamilyCompositionCommand, Result<FamilyCompositionResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IFamilyCompositionRepository _familyCompositionRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de atualização de composição familiar.</summary>
    public UpdateFamilyCompositionCommandHandler(
        IPersonRepository personRepository,
        IFamilyCompositionRepository familyCompositionRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _familyCompositionRepository = familyCompositionRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<FamilyCompositionResponse>> Handle(
        UpdateFamilyCompositionCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<FamilyCompositionResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var existing = await _familyCompositionRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        if (existing is null)
        {
            var createResult = Sigis.Domain.Entities.FamilyComposition.Create(
                request.PersonId, _dateTimeProvider.UtcNow, request.FatherName, request.FatherEducation,
                request.FatherOccupation, request.MotherName, request.MotherEducation, request.MotherOccupation,
                request.SiblingsCount, request.SiblingsAges, request.HouseholdMembersCount,
                request.ParentsMaritalStatus, request.FiliationType, request.PlannedPregnancy,
                request.PregnanciesCount, request.AbortionsCount, request.PregnancyHealthIssue,
                request.DeliveryType, request.MedicationDuringPregnancy);

            if (createResult.IsFailure)
                return Result<FamilyCompositionResponse>.Failure(createResult.Error);

            await _familyCompositionRepository.AddAsync(createResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<FamilyCompositionResponse>.Success(FamilyCompositionMapper.ToResponse(createResult.Value));
        }

        var updateResult = existing.UpdateData(
            request.FatherName, request.FatherEducation, request.FatherOccupation, request.MotherName,
            request.MotherEducation, request.MotherOccupation, request.SiblingsCount, request.SiblingsAges,
            request.HouseholdMembersCount, request.ParentsMaritalStatus);

        if (updateResult.IsFailure)
            return Result<FamilyCompositionResponse>.Failure(updateResult.Error);

        var pregnancyResult = existing.SetPregnancyInfo(
            request.FiliationType, request.PlannedPregnancy, request.PregnanciesCount, request.AbortionsCount,
            request.PregnancyHealthIssue, request.DeliveryType, request.MedicationDuringPregnancy);

        if (pregnancyResult.IsFailure)
            return Result<FamilyCompositionResponse>.Failure(pregnancyResult.Error);

        await _familyCompositionRepository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<FamilyCompositionResponse>.Success(FamilyCompositionMapper.ToResponse(existing));
    }
}
