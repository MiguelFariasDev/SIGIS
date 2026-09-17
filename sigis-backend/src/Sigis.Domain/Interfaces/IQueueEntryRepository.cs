using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para o agregado <see cref="QueueEntry"/>.</summary>
public interface IQueueEntryRepository
{
    /// <summary>Busca uma entrada de fila pelo identificador.</summary>
    /// <param name="id">Identificador da entrada de fila.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A entrada encontrada, ou <see langword="null"/> quando não existir.</returns>
    Task<QueueEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Lista as entradas de fila ativas de uma unidade de serviço.</summary>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de entradas ativas da unidade, podendo ser vazia.</returns>
    Task<IReadOnlyList<QueueEntry>> GetActiveByUnitAsync(Guid unitId, CancellationToken cancellationToken);

    /// <summary>
    /// Lista todas as entradas de fila de uma unidade de serviço, independente
    /// da situação — usado pela consulta de fila com filtros (status,
    /// prioridade, especialidade), que precisa enxergar além das entradas
    /// ativas cobertas por <see cref="GetActiveByUnitAsync"/>.
    /// </summary>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de todas as entradas da unidade, podendo ser vazia.</returns>
    Task<IReadOnlyList<QueueEntry>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se já existe uma entrada de fila ativa para a pessoa na
    /// unidade informada (RN01 — unicidade de fila ativa por pessoa e unidade).
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns><see langword="true"/> quando já existir uma entrada ativa.</returns>
    Task<bool> HasActiveEntryAsync(Guid personId, Guid unitId, CancellationToken cancellationToken);

    /// <summary>Adiciona uma nova entrada de fila ao repositório.</summary>
    /// <param name="queueEntry">Entrada de fila a ser adicionada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(QueueEntry queueEntry, CancellationToken cancellationToken);

    /// <summary>
    /// Busca entradas de fila (em qualquer situação) filtradas opcionalmente
    /// por unidade e período de entrada — usado pelo painel de indicadores.
    /// </summary>
    /// <param name="unitId">Identificador da unidade de serviço, opcional.</param>
    /// <param name="from">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="to">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de entradas de fila que atendem aos filtros informados, podendo ser vazia.</returns>
    Task<IReadOnlyList<QueueEntry>> SearchAsync(
        Guid? unitId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de uma entrada de fila já existente no repositório.</summary>
    /// <param name="queueEntry">Entrada de fila com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(QueueEntry queueEntry, CancellationToken cancellationToken);

    /// <summary>
    /// Reatribui todas as entradas de fila de uma pessoa para outra, em
    /// massa — usado na mesclagem de cadastros duplicados (RN02).
    /// </summary>
    /// <param name="fromPersonId">Identificador do cadastro de origem.</param>
    /// <param name="toPersonId">Identificador do cadastro de destino.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Quantidade de entradas de fila reatribuídas.</returns>
    Task<int> ReassignPersonAsync(Guid fromPersonId, Guid toPersonId, CancellationToken cancellationToken);
}
