using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="PersonConsent"/>.</summary>
public interface IPersonConsentRepository
{
    /// <summary>Lista todos os consentimentos (ativos e revogados) de uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de consentimentos da pessoa, podendo ser vazia.</returns>
    Task<IReadOnlyList<PersonConsent>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Busca o consentimento ativo de um tipo específico para a pessoa, quando houver (RN-PC01).</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="type">Tipo/finalidade do consentimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O consentimento ativo, ou <see langword="null"/> quando não houver.</returns>
    Task<PersonConsent?> GetActiveByPersonAndTypeAsync(
        Guid personId, ConsentType type, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo consentimento ao repositório.</summary>
    /// <param name="consent">Consentimento a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(PersonConsent consent, CancellationToken cancellationToken);

    /// <summary>Atualiza um consentimento já existente no repositório (ex.: revogação).</summary>
    /// <param name="consent">Consentimento com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(PersonConsent consent, CancellationToken cancellationToken);
}
