using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para o agregado <see cref="Referral"/>.</summary>
public interface IReferralRepository
{
    /// <summary>Busca um encaminhamento pelo identificador.</summary>
    /// <param name="id">Identificador do encaminhamento.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O encaminhamento encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<Referral?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Lista o histórico de encaminhamentos de uma pessoa em todas as unidades da rede.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de encaminhamentos da pessoa, podendo ser vazia.</returns>
    Task<IReadOnlyList<Referral>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Lista os encaminhamentos pendentes ou aceitos, direcionados a uma unidade de destino.</summary>
    /// <param name="destinationUnitId">Identificador da unidade de destino.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de encaminhamentos ativos direcionados à unidade, podendo ser vazia.</returns>
    Task<IReadOnlyList<Referral>> GetActiveByDestinationUnitAsync(
        Guid destinationUnitId, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo encaminhamento ao repositório.</summary>
    /// <param name="referral">Encaminhamento a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(Referral referral, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de um encaminhamento já existente no repositório.</summary>
    /// <param name="referral">Encaminhamento com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(Referral referral, CancellationToken cancellationToken);
}
