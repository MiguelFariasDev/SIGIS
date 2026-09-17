using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IQueueEntryRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class QueueEntryRepository : IQueueEntryRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="QueueEntry"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public QueueEntryRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<QueueEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.QueueEntries.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<QueueEntry>> GetActiveByUnitAsync(Guid unitId, CancellationToken cancellationToken)
        => await _context.QueueEntries
            .Where(q => q.UnitId == unitId
                && (q.Status == QueueStatus.Waiting || q.Status == QueueStatus.InAttendance))
            .OrderBy(q => q.Priority)
            .ThenBy(q => q.EnteredAt)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> HasActiveEntryAsync(Guid personId, Guid unitId, CancellationToken cancellationToken)
        => await _context.QueueEntries.AnyAsync(
            q => q.PersonId == personId
                && q.UnitId == unitId
                && (q.Status == QueueStatus.Waiting || q.Status == QueueStatus.InAttendance),
            cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(QueueEntry queueEntry, CancellationToken cancellationToken)
        => await _context.QueueEntries.AddAsync(queueEntry, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(QueueEntry queueEntry, CancellationToken cancellationToken)
    {
        _context.QueueEntries.Update(queueEntry);
        return Task.CompletedTask;
    }
}
