using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.ClinicalProfile;

/// <summary>Processa a consulta do perfil clínico NASF de uma pessoa.</summary>
public sealed class GetClinicalProfileQueryHandler : IRequestHandler<GetClinicalProfileQuery, Result<PersonClinicalProfileResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IPersonClinicalProfileRepository _clinicalProfileRepository;

    /// <summary>Cria o handler de consulta de perfil clínico.</summary>
    public GetClinicalProfileQueryHandler(IPersonRepository personRepository, IPersonClinicalProfileRepository clinicalProfileRepository)
    {
        _personRepository = personRepository;
        _clinicalProfileRepository = clinicalProfileRepository;
    }

    /// <inheritdoc />
    public async Task<Result<PersonClinicalProfileResponse>> Handle(GetClinicalProfileQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<PersonClinicalProfileResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var profile = await _clinicalProfileRepository.GetByPersonAsync(request.PersonId, cancellationToken);
        if (profile is null)
            return Result<PersonClinicalProfileResponse>.Failure(CommonErrors.NotFound);

        return Result<PersonClinicalProfileResponse>.Success(new PersonClinicalProfileResponse(
            profile.Id, profile.PersonId, profile.MedicalRecordNumber, profile.ClinicalHypothesis,
            profile.ApsReferenceUnitId, profile.CreatedAt, profile.UpdatedAt));
    }
}
