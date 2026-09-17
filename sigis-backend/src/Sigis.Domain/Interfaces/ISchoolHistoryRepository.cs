using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="SchoolHistory"/>.</summary>
public interface ISchoolHistoryRepository
{
    /// <summary>Busca um registro de histórico escolar pelo identificador.</summary>
    /// <param name="id">Identificador do registro.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O registro encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<SchoolHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Lista o histórico escolar completo de uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de registros da pessoa, podendo ser vazia.</returns>
    Task<IReadOnlyList<SchoolHistory>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Busca o registro escolar atualmente ativo de uma pessoa, quando houver (RN-SH01).</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O registro ativo, ou <see langword="null"/> quando não houver.</returns>
    Task<SchoolHistory?> GetActiveByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo registro de histórico escolar ao repositório.</summary>
    /// <param name="schoolHistory">Registro a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(SchoolHistory schoolHistory, CancellationToken cancellationToken);

    /// <summary>Atualiza um registro de histórico escolar já existente no repositório.</summary>
    /// <param name="schoolHistory">Registro com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(SchoolHistory schoolHistory, CancellationToken cancellationToken);
}
