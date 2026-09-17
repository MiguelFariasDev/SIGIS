namespace Sigis.Domain.Interfaces;

/// <summary>
/// Coordena a persistência transacional das alterações realizadas nos
/// repositórios durante o processamento de um caso de uso.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persiste todas as alterações pendentes em uma única transação.</summary>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Número de registros afetados pela operação.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
