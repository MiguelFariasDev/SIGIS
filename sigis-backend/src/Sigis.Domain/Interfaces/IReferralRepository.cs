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

    /// <summary>Lista todos os encaminhamentos (qualquer situação) enviados por uma unidade de origem.</summary>
    /// <param name="originUnitId">Identificador da unidade de origem.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de encaminhamentos enviados pela unidade, mais recentes primeiro, podendo ser vazia.</returns>
    Task<IReadOnlyList<Referral>> GetByOriginUnitAsync(Guid originUnitId, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo encaminhamento ao repositório.</summary>
    /// <param name="referral">Encaminhamento a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(Referral referral, CancellationToken cancellationToken);

    /// <summary>
    /// Busca encaminhamentos (em qualquer situação) filtrados opcionalmente
    /// por unidade (origem ou destino) e período — usado pelo painel de
    /// indicadores.
    /// </summary>
    /// <param name="unitId">Identificador da unidade de serviço (origem ou destino), opcional.</param>
    /// <param name="from">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="to">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de encaminhamentos que atendem aos filtros informados, podendo ser vazia.</returns>
    Task<IReadOnlyList<Referral>> SearchAsync(
        Guid? unitId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de um encaminhamento já existente no repositório.</summary>
    /// <param name="referral">Encaminhamento com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(Referral referral, CancellationToken cancellationToken);

    /// <summary>
    /// Reatribui todos os encaminhamentos de uma pessoa para outra, em
    /// massa — usado na mesclagem de cadastros duplicados (RN02).
    /// </summary>
    /// <param name="fromPersonId">Identificador do cadastro de origem.</param>
    /// <param name="toPersonId">Identificador do cadastro de destino.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Quantidade de encaminhamentos reatribuídos.</returns>
    Task<int> ReassignPersonAsync(Guid fromPersonId, Guid toPersonId, CancellationToken cancellationToken);
}
