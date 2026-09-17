using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.ClinicalProfile;

/// <summary>
/// Processa a criação (se ainda não existir) ou atualização do perfil
/// clínico NASF de uma pessoa (get-or-create — relação 1:1), aplicando
/// RN-CP02 (prontuário único por unidade) e RN-CP04 (APS deve ser unidade
/// da Secretaria de Saúde) antes de delegar as demais validações à entidade.
/// </summary>
/// <remarks>
/// RN-CP03 (H.D. obrigatória para pacientes com atendimento em NASF) não é
/// verificada nesta versão — exigiria consultar o histórico de atendimentos
/// da pessoa em outro módulo (Attendances), fora do escopo deste caso de uso.
/// </remarks>
public sealed class UpdateClinicalProfileCommandHandler
    : IRequestHandler<UpdateClinicalProfileCommand, Result<PersonClinicalProfileResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IPersonClinicalProfileRepository _clinicalProfileRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de atualização de perfil clínico.</summary>
    public UpdateClinicalProfileCommandHandler(
        IPersonRepository personRepository,
        IPersonClinicalProfileRepository clinicalProfileRepository,
        IServiceUnitRepository serviceUnitRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _clinicalProfileRepository = clinicalProfileRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<PersonClinicalProfileResponse>> Handle(
        UpdateClinicalProfileCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<PersonClinicalProfileResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        if (request.ApsReferenceUnitId.HasValue)
        {
            var unit = await _serviceUnitRepository.GetByIdAsync(request.ApsReferenceUnitId.Value, cancellationToken);
            if (unit is null || !unit.BelongsToHealth())
                return Result<PersonClinicalProfileResponse>.Failure(ClinicalProfileErrors.ApsDeveSerSaude);

            if (!string.IsNullOrWhiteSpace(request.MedicalRecordNumber))
            {
                var duplicate = await _clinicalProfileRepository.ExistsMedicalRecordNumberInUnitAsync(
                    request.ApsReferenceUnitId.Value, request.MedicalRecordNumber, cancellationToken);
                if (duplicate)
                    return Result<PersonClinicalProfileResponse>.Failure(ClinicalProfileErrors.ProntuarioDuplicado);
            }
        }

        var existing = await _clinicalProfileRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        if (existing is null)
        {
            var createResult = Sigis.Domain.Entities.PersonClinicalProfile.Create(
                request.PersonId, _dateTimeProvider.UtcNow, request.MedicalRecordNumber,
                request.ClinicalHypothesis, request.ApsReferenceUnitId);

            if (createResult.IsFailure)
                return Result<PersonClinicalProfileResponse>.Failure(createResult.Error);

            await _clinicalProfileRepository.AddAsync(createResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var created = createResult.Value;
            return Result<PersonClinicalProfileResponse>.Success(new PersonClinicalProfileResponse(
                created.Id, created.PersonId, created.MedicalRecordNumber, created.ClinicalHypothesis,
                created.ApsReferenceUnitId, created.CreatedAt, created.UpdatedAt));
        }

        existing.UpdateData(request.MedicalRecordNumber, request.ClinicalHypothesis, request.ApsReferenceUnitId);

        await _clinicalProfileRepository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PersonClinicalProfileResponse>.Success(new PersonClinicalProfileResponse(
            existing.Id, existing.PersonId, existing.MedicalRecordNumber, existing.ClinicalHypothesis,
            existing.ApsReferenceUnitId, existing.CreatedAt, existing.UpdatedAt));
    }
}
