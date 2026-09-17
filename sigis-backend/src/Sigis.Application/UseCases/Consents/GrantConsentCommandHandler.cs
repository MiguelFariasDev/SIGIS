using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Consents;

/// <summary>
/// Processa a concessão de um novo consentimento LGPD para uma pessoa,
/// aplicando RN-PC01 (um único consentimento ativo por tipo por pessoa)
/// antes de delegar as demais validações à entidade.
/// </summary>
public sealed class GrantConsentCommandHandler : IRequestHandler<GrantConsentCommand, Result<PersonConsentResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IPersonConsentRepository _personConsentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de concessão de consentimento.</summary>
    public GrantConsentCommandHandler(
        IPersonRepository personRepository,
        IPersonConsentRepository personConsentRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _personConsentRepository = personConsentRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<PersonConsentResponse>> Handle(GrantConsentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = new GrantConsentCommandValidator().Validate(request);
        if (!validationResult.IsValid)
            return Result<PersonConsentResponse>.Failure(new Error("CONSENT_VALIDATION", validationResult.Errors[0].ErrorMessage));

        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<PersonConsentResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var active = await _personConsentRepository.GetActiveByPersonAndTypeAsync(request.PersonId, request.Type, cancellationToken);
        if (active is not null)
            return Result<PersonConsentResponse>.Failure(PersonConsentErrors.ConsentimentoAtivoJaExiste);

        var now = _dateTimeProvider.UtcNow;
        var createResult = Sigis.Domain.Entities.PersonConsent.Create(
            request.PersonId, request.Type, now, request.Version!, now, request.GrantedByGuardianId, request.Evidence);

        if (createResult.IsFailure)
            return Result<PersonConsentResponse>.Failure(createResult.Error);

        var consent = createResult.Value;
        await _personConsentRepository.AddAsync(consent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PersonConsentResponse>.Success(new PersonConsentResponse(
            consent.Id, consent.PersonId, consent.Type, consent.Granted, consent.GrantedAt,
            consent.GrantedByGuardianId, consent.RevokedAt, consent.Version, consent.Evidence, consent.CreatedAt));
    }
}
