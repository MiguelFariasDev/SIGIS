using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="Professional"/>.</summary>
public interface IProfessionalRepository
{
    /// <summary>Busca um profissional pelo identificador.</summary>
    /// <param name="id">Identificador do profissional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O profissional encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<Professional?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Lista os profissionais vinculados a uma unidade de serviço.</summary>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de profissionais da unidade, podendo ser vazia.</returns>
    Task<IReadOnlyList<Professional>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo profissional ao repositório.</summary>
    /// <param name="professional">Profissional a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(Professional professional, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de um profissional já existente no repositório.</summary>
    /// <param name="professional">Profissional com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(Professional professional, CancellationToken cancellationToken);
}
