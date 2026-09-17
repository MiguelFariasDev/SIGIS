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

    /// <summary>Atualiza os dados de uma entrada de fila já existente no repositório.</summary>
    /// <param name="queueEntry">Entrada de fila com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(QueueEntry queueEntry, CancellationToken cancellationToken);
}
