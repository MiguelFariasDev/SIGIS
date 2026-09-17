using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="ConcurrentTreatment"/>.</summary>
public interface IConcurrentTreatmentRepository
{
    /// <summary>Lista os atendimentos concomitantes de uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de atendimentos concomitantes, podendo ser vazia.</returns>
    Task<IReadOnlyList<ConcurrentTreatment>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Busca um atendimento concomitante pelo identificador.</summary>
    /// <param name="id">Identificador do atendimento concomitante.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O atendimento encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<ConcurrentTreatment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo atendimento concomitante ao repositório.</summary>
    /// <param name="treatment">Atendimento a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(ConcurrentTreatment treatment, CancellationToken cancellationToken);

    /// <summary>Remove um atendimento concomitante do repositório.</summary>
    /// <param name="treatment">Atendimento a ser removido.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task RemoveAsync(ConcurrentTreatment treatment, CancellationToken cancellationToken);
}
