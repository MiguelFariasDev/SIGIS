using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="FamilyComposition"/> (1:1 com pessoa).</summary>
public interface IFamilyCompositionRepository
{
    /// <summary>Busca a composição familiar de uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A composição familiar, ou <see langword="null"/> quando ainda não cadastrada.</returns>
    Task<FamilyComposition?> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Adiciona a composição familiar de uma pessoa.</summary>
    /// <param name="familyComposition">Registro a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(FamilyComposition familyComposition, CancellationToken cancellationToken);

    /// <summary>Atualiza a composição familiar já existente no repositório.</summary>
    /// <param name="familyComposition">Registro com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(FamilyComposition familyComposition, CancellationToken cancellationToken);
}
