using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Monta o <see cref="DuplicateAlertResponse"/> de um alerta, resolvendo os nomes das duas pessoas envolvidas.</summary>
public static class DuplicateAlertResponseMapper
{
    /// <summary>Mapeia um <see cref="DuplicateAlert"/> para <see cref="DuplicateAlertResponse"/>.</summary>
    public static async Task<DuplicateAlertResponse> MapAsync(
        DuplicateAlert alert, IPersonRepository personRepository, CancellationToken cancellationToken)
    {
        var person1 = await personRepository.GetByIdAsync(alert.PersonId1, cancellationToken);
        var person2 = await personRepository.GetByIdAsync(alert.PersonId2, cancellationToken);

        return new DuplicateAlertResponse(
            alert.Id,
            alert.PersonId1,
            person1?.Name.Value ?? "Pessoa não encontrada",
            alert.PersonId2,
            person2?.Name.Value ?? "Pessoa não encontrada",
            alert.SimilarityScore,
            alert.CreatedAt);
    }
}
