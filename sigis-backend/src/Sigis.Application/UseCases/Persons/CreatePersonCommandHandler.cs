using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;

namespace Sigis.Application.UseCases.Persons;

/// <summary>
/// Processa o cadastro de uma nova pessoa: valida os dados, verifica
/// duplicidade exata (CNS/CPF) e aproximada (RN02, camada 3 — probabilística
/// via <c>pg_trgm</c>), persiste o cadastro e abre um <see cref="DuplicateAlert"/>
/// para cada candidato aproximado encontrado, para revisão de um coordenador.
/// </summary>
public sealed class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<CreatePersonResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de cadastro de pessoa.</summary>
    public CreatePersonCommandHandler(
        IPersonRepository personRepository,
        IDuplicateAlertRepository duplicateAlertRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _duplicateAlertRepository = duplicateAlertRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<CreatePersonResponse>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var validationResult = new CreatePersonCommandValidator().Validate(request);
        if (!validationResult.IsValid)
            return Result<CreatePersonResponse>.Failure(new Error("PERSON_VALIDATION", validationResult.Errors[0].ErrorMessage));

        var nameResult = PersonName.Create(request.Name);
        if (nameResult.IsFailure)
            return Result<CreatePersonResponse>.Failure(nameResult.Error);

        Cns? cns = null;
        if (!string.IsNullOrWhiteSpace(request.Cns))
        {
            var cnsResult = Cns.Create(request.Cns);
            if (cnsResult.IsFailure)
                return Result<CreatePersonResponse>.Failure(cnsResult.Error);
            cns = cnsResult.Value;
        }

        Cpf? cpf = null;
        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            var cpfResult = Cpf.Create(request.Cpf);
            if (cpfResult.IsFailure)
                return Result<CreatePersonResponse>.Failure(cpfResult.Error);
            cpf = cpfResult.Value;
        }

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phoneResult = PhoneNumber.Create(request.Phone);
            if (phoneResult.IsFailure)
                return Result<CreatePersonResponse>.Failure(phoneResult.Error);
            phone = phoneResult.Value;
        }

        EmailAddress? email = null;
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailResult = EmailAddress.Create(request.Email);
            if (emailResult.IsFailure)
                return Result<CreatePersonResponse>.Failure(emailResult.Error);
            email = emailResult.Value;
        }

        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Street))
        {
            var addressResult = Address.Create(
                request.Street, request.Number, request.Neighborhood, request.City, request.State, request.ZipCode);
            if (addressResult.IsFailure)
                return Result<CreatePersonResponse>.Failure(addressResult.Error);
            address = addressResult.Value;
        }

        // Camada 1 — CNS exato (RN02).
        if (cns is not null && await _personRepository.GetByCnsAsync(cns, cancellationToken) is not null)
            return Result<CreatePersonResponse>.Failure(PersonErrors.CnsDuplicado);

        // Camada 2 — CPF exato (RN02).
        if (cpf is not null && await _personRepository.GetByCpfAsync(cpf, cancellationToken) is not null)
            return Result<CreatePersonResponse>.Failure(PersonErrors.CpfDuplicado);

        var personResult = Person.Create(
            nameResult.Value, request.BirthDate, _dateTimeProvider.Today, cns, cpf,
            request.MotherName, request.Gender, request.RaceColor, phone, email, address);
        if (personResult.IsFailure)
            return Result<CreatePersonResponse>.Failure(personResult.Error);

        var person = personResult.Value;
        await _personRepository.AddAsync(person, cancellationToken);

        // Camada 3 — probabilística (nome aproximado + nascimento exato, via
        // pg_trgm na infraestrutura). Não bloqueia o cadastro — apenas abre
        // um alerta para revisão de um coordenador (ver CreatePersonResponse).
        var candidates = await _personRepository.FindDuplicateCandidatesAsync(nameResult.Value, request.BirthDate, cancellationToken);

        var candidateSummaries = new List<PersonSummary>();
        foreach (var candidate in candidates)
        {
            candidateSummaries.Add(new PersonSummary(candidate.Id, candidate.Name.Value, candidate.BirthDate, candidate.MotherName));

            // Similaridade fixa no limiar mínimo: o score exato do matching
            // por trigram é calculado em SQL (pg_trgm) na consulta acima, mas
            // FindDuplicateCandidatesAsync não o retorna hoje — usar o
            // limiar mínimo é uma aproximação conservadora até essa consulta
            // expor o score real.
            var alertResult = DuplicateAlert.Create(
                person.Id, candidate.Id, DuplicateAlert.MinimumSimilarityThreshold, _dateTimeProvider.UtcNow);
            if (alertResult.IsSuccess)
                await _duplicateAlertRepository.AddAsync(alertResult.Value, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreatePersonResponse>.Success(new CreatePersonResponse(person.Id, candidateSummaries));
    }
}
