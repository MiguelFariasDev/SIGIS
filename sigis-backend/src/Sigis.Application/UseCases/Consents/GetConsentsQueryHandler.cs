using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Processa a consulta global de consentimentos LGPD (painel DPO/coordenador).</summary>
public sealed class GetConsentsQueryHandler
    : IRequestHandler<GetConsentsQuery, Result<IReadOnlyList<PersonConsentWithPersonResponse>>>
{
    private readonly IPersonConsentRepository _personConsentRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de consulta global de consentimentos.</summary>
    public GetConsentsQueryHandler(IPersonConsentRepository personConsentRepository, IPersonRepository personRepository)
    {
        _personConsentRepository = personConsentRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PersonConsentWithPersonResponse>>> Handle(
        GetConsentsQuery request, CancellationToken cancellationToken)
    {
        var consents = await _personConsentRepository.SearchAsync(
            request.PersonId, request.Type, request.Revoked, request.From, request.To, cancellationToken);

        var responses = new List<PersonConsentWithPersonResponse>(consents.Count);
        foreach (var consent in consents)
        {
            var person = await _personRepository.GetByIdAsync(consent.PersonId, cancellationToken);
            responses.Add(new PersonConsentWithPersonResponse(
                consent.Id,
                consent.PersonId,
                person?.Name.Value ?? "Pessoa não encontrada",
                consent.Type,
                consent.Granted,
                consent.GrantedAt,
                consent.GrantedByGuardianId,
                consent.RevokedAt,
                consent.Version,
                consent.Evidence,
                consent.CreatedAt));
        }

        return Result<IReadOnlyList<PersonConsentWithPersonResponse>>.Success(responses);
    }
}
