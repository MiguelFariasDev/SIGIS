using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="AccessLog"/>.</summary>
public interface IAccessLogRepository
{
    /// <summary>Lista os registros de auditoria de acesso a uma pessoa (RF13).</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de registros de acesso à pessoa, podendo ser vazia.</returns>
    Task<IReadOnlyList<AccessLog>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>Lista os registros de auditoria de acesso realizados por um profissional.</summary>
    /// <param name="professionalId">Identificador do profissional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de registros de acesso do profissional, podendo ser vazia.</returns>
    Task<IReadOnlyList<AccessLog>> GetByProfessionalAsync(Guid professionalId, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo registro de auditoria de acesso ao repositório.</summary>
    /// <param name="accessLog">Registro de acesso a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(AccessLog accessLog, CancellationToken cancellationToken);

    /// <summary>
    /// Busca registros de auditoria de acesso filtrados por pessoa,
    /// profissional e/ou período — usado pela controller de auditoria (RF13).
    /// </summary>
    /// <param name="personId">Identificador da pessoa, opcional.</param>
    /// <param name="professionalId">Identificador do profissional, opcional.</param>
    /// <param name="from">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="to">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de registros de acesso que atendem aos filtros informados, podendo ser vazia.</returns>
    Task<IReadOnlyList<AccessLog>> SearchAsync(
        Guid? personId, Guid? professionalId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

    /// <summary>Lista todos os registros de auditoria classificados como acesso entre unidades (RNF02, RF13).</summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de registros de acesso cross-unidade, podendo ser vazia.</returns>
    Task<IReadOnlyList<AccessLog>> GetCrossUnitAsync(CancellationToken cancellationToken);
}
