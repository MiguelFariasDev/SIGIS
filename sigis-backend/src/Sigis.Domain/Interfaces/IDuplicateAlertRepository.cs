using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para o agregado <see cref="DuplicateAlert"/>.</summary>
public interface IDuplicateAlertRepository
{
    /// <summary>Busca um alerta de duplicidade pelo identificador.</summary>
    /// <param name="id">Identificador do alerta.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O alerta encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<DuplicateAlert?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Lista os alertas de duplicidade pendentes de revisão (RF04).</summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de alertas pendentes, podendo ser vazia.</returns>
    Task<IReadOnlyList<DuplicateAlert>> GetPendingAsync(CancellationToken cancellationToken);

    /// <summary>Adiciona um novo alerta de duplicidade ao repositório.</summary>
    /// <param name="alert">Alerta a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(DuplicateAlert alert, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de um alerta de duplicidade já existente no repositório.</summary>
    /// <param name="alert">Alerta com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(DuplicateAlert alert, CancellationToken cancellationToken);
}
