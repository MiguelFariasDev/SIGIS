using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IDuplicateAlertRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class DuplicateAlertRepository : IDuplicateAlertRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="DuplicateAlert"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public DuplicateAlertRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<DuplicateAlert?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.DuplicateAlerts.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<DuplicateAlert>> GetPendingAsync(CancellationToken cancellationToken)
        => await _context.DuplicateAlerts
            .Where(d => d.Status == DuplicateAlertStatus.Pending)
            .OrderByDescending(d => d.SimilarityScore)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(DuplicateAlert alert, CancellationToken cancellationToken)
        => await _context.DuplicateAlerts.AddAsync(alert, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(DuplicateAlert alert, CancellationToken cancellationToken)
    {
        _context.DuplicateAlerts.Update(alert);
        return Task.CompletedTask;
    }
}
