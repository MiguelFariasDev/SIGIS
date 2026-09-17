using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="DevelopmentMilestones"/> (1:1 com pessoa).</summary>
public interface IDevelopmentMilestonesRepository
{
    /// <summary>Busca os marcos de desenvolvimento de uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Os marcos de desenvolvimento, ou <see langword="null"/> quando ainda não cadastrados.</returns>
    Task<DevelopmentMilestones?> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Adiciona os marcos de desenvolvimento de uma pessoa.</summary>
    /// <param name="milestones">Registro a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(DevelopmentMilestones milestones, CancellationToken cancellationToken);

    /// <summary>Atualiza os marcos de desenvolvimento já existentes no repositório.</summary>
    /// <param name="milestones">Registro com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(DevelopmentMilestones milestones, CancellationToken cancellationToken);
}
