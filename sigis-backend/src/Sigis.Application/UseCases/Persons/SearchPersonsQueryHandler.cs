using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Processa a busca de pessoas por nome, limitando a quantidade de resultados retornados.</summary>
public sealed class SearchPersonsQueryHandler : IRequestHandler<SearchPersonsQuery, Result<IReadOnlyList<PersonSummary>>>
{
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de busca de pessoas.</summary>
    public SearchPersonsQueryHandler(IPersonRepository personRepository) => _personRepository = personRepository;

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PersonSummary>>> Handle(SearchPersonsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Term))
            return Result<IReadOnlyList<PersonSummary>>.Success([]);

        var limit = request.Limit <= 0 ? 20 : request.Limit;

        var people = await _personRepository.SearchByNameAsync(request.Term, cancellationToken);

        IReadOnlyList<PersonSummary> results = people
            .Take(limit)
            .Select(p => new PersonSummary(p.Id, p.Name.Value, p.BirthDate, p.MotherName))
            .ToList();

        return Result<IReadOnlyList<PersonSummary>>.Success(results);
    }
}
