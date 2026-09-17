using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="ServiceUnit"/>.</summary>
public interface IServiceUnitRepository
{
    /// <summary>Busca uma unidade de serviço pelo identificador.</summary>
    /// <param name="id">Identificador da unidade.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A unidade encontrada, ou <see langword="null"/> quando não existir.</returns>
    Task<ServiceUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Lista todas as unidades de serviço cadastradas.</summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de unidades de serviço, podendo ser vazia.</returns>
    Task<IReadOnlyList<ServiceUnit>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Adiciona uma nova unidade de serviço ao repositório.</summary>
    /// <param name="unit">Unidade a ser adicionada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(ServiceUnit unit, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de uma unidade de serviço já existente no repositório.</summary>
    /// <param name="unit">Unidade com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(ServiceUnit unit, CancellationToken cancellationToken);
}
