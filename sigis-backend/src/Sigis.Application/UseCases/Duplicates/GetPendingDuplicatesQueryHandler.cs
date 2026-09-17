using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Processa a consulta paginada dos alertas de duplicidade pendentes de revisão (RF04).</summary>
public sealed class GetPendingDuplicatesQueryHandler
    : IRequestHandler<GetPendingDuplicatesQuery, Result<IReadOnlyList<DuplicateAlertResponse>>>
{
    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de consulta de duplicidades pendentes.</summary>
    public GetPendingDuplicatesQueryHandler(IDuplicateAlertRepository duplicateAlertRepository, IPersonRepository personRepository)
    {
        _duplicateAlertRepository = duplicateAlertRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DuplicateAlertResponse>>> Handle(
        GetPendingDuplicatesQuery request, CancellationToken cancellationToken)
    {
        var skip = Math.Max(request.Skip, 0);
        var take = request.Take <= 0 ? 20 : request.Take;

        var pending = await _duplicateAlertRepository.GetPendingAsync(cancellationToken);
        var page = pending.Skip(skip).Take(take).ToList();

        var responses = new List<DuplicateAlertResponse>(page.Count);
        foreach (var alert in page)
        {
            var person1 = await _personRepository.GetByIdAsync(alert.PersonId1, cancellationToken);
            var person2 = await _personRepository.GetByIdAsync(alert.PersonId2, cancellationToken);

            responses.Add(new DuplicateAlertResponse(
                alert.Id,
                alert.PersonId1,
                person1?.Name.Value ?? "Pessoa não encontrada",
                alert.PersonId2,
                person2?.Name.Value ?? "Pessoa não encontrada",
                alert.SimilarityScore,
                alert.CreatedAt));
        }

        return Result<IReadOnlyList<DuplicateAlertResponse>>.Success(responses);
    }
}
