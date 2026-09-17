using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="PersonConsent"/>.</summary>
public interface IPersonConsentRepository
{
    /// <summary>Busca um consentimento pelo identificador.</summary>
    /// <param name="id">Identificador do consentimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O consentimento encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<PersonConsent?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

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

    /// <summary>
    /// Busca consentimentos de todas as pessoas, filtrados opcionalmente por
    /// pessoa, tipo, situação de revogação e período de concessão — usado
    /// pelo painel global de consentimentos (DPO/coordenador).
    /// </summary>
    /// <param name="personId">Identificador da pessoa, opcional.</param>
    /// <param name="type">Tipo/finalidade do consentimento, opcional.</param>
    /// <param name="revoked">
    /// <see langword="true"/> para apenas revogados, <see langword="false"/>
    /// para apenas ativos, ou <see langword="null"/> para não filtrar.
    /// </param>
    /// <param name="from">Início do período de concessão (UTC, inclusive), opcional.</param>
    /// <param name="to">Fim do período de concessão (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de consentimentos que atendem aos filtros informados, podendo ser vazia.</returns>
    Task<IReadOnlyList<PersonConsent>> SearchAsync(
        Guid? personId, ConsentType? type, bool? revoked, DateTime? from, DateTime? to,
        CancellationToken cancellationToken);

    /// <summary>Adiciona um novo consentimento ao repositório.</summary>
    /// <param name="consent">Consentimento a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(PersonConsent consent, CancellationToken cancellationToken);

    /// <summary>Atualiza um consentimento já existente no repositório (ex.: revogação).</summary>
    /// <param name="consent">Consentimento com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(PersonConsent consent, CancellationToken cancellationToken);
}
