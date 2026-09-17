using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Processa a consulta de todos os consentimentos de uma pessoa.</summary>
public sealed class GetPersonConsentsQueryHandler : IRequestHandler<GetPersonConsentsQuery, Result<IReadOnlyList<PersonConsentResponse>>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IPersonConsentRepository _personConsentRepository;

    /// <summary>Cria o handler de consulta de consentimentos.</summary>
    public GetPersonConsentsQueryHandler(IPersonRepository personRepository, IPersonConsentRepository personConsentRepository)
    {
        _personRepository = personRepository;
        _personConsentRepository = personConsentRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PersonConsentResponse>>> Handle(
        GetPersonConsentsQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<IReadOnlyList<PersonConsentResponse>>.Failure(PersonErrors.PessoaNaoEncontrada);

        var consents = await _personConsentRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var response = consents
            .Select(c => new PersonConsentResponse(
                c.Id, c.PersonId, c.Type, c.Granted, c.GrantedAt, c.GrantedByGuardianId, c.RevokedAt, c.Version, c.Evidence, c.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<PersonConsentResponse>>.Success(response);
    }
}
