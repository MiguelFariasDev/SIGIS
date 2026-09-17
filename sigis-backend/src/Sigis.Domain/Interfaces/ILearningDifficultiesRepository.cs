using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="LearningDifficulties"/>.</summary>
public interface ILearningDifficultiesRepository
{
    /// <summary>Lista as dificuldades de aprendizagem registradas para uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de dificuldades da pessoa, podendo ser vazia.</returns>
    Task<IReadOnlyList<LearningDifficulties>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Busca o registro de um tipo específico de dificuldade para a pessoa, quando existir (RN-LD01).</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="type">Tipo de dificuldade.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O registro encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<LearningDifficulties?> GetByPersonAndTypeAsync(
        Guid personId, LearningDifficultyType type, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo registro de dificuldade de aprendizagem ao repositório.</summary>
    /// <param name="learningDifficulty">Registro a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(LearningDifficulties learningDifficulty, CancellationToken cancellationToken);

    /// <summary>Atualiza um registro de dificuldade de aprendizagem já existente no repositório.</summary>
    /// <param name="learningDifficulty">Registro com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(LearningDifficulties learningDifficulty, CancellationToken cancellationToken);
}
