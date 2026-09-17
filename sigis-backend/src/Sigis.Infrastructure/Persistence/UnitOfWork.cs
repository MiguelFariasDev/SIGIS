using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence;

/// <summary>
/// Implementação de <see cref="IUnitOfWork"/> que delega a persistência
/// transacional ao <see cref="SigisDbContext"/> do Entity Framework Core.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SigisDbContext _context;

    /// <summary>Cria a unidade de trabalho.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public UnitOfWork(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}
