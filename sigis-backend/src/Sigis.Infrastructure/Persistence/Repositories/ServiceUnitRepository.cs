using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IServiceUnitRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class ServiceUnitRepository : IServiceUnitRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="ServiceUnit"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public ServiceUnitRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ServiceUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.ServiceUnits.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceUnit>> GetAllAsync(CancellationToken cancellationToken)
        => await _context.ServiceUnits.OrderBy(u => u.Name).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(ServiceUnit unit, CancellationToken cancellationToken)
        => await _context.ServiceUnits.AddAsync(unit, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(ServiceUnit unit, CancellationToken cancellationToken)
    {
        _context.ServiceUnits.Update(unit);
        return Task.CompletedTask;
    }
}
