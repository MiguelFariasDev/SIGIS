using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IConcurrentTreatmentRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class ConcurrentTreatmentRepository : IConcurrentTreatmentRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="ConcurrentTreatment"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public ConcurrentTreatmentRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ConcurrentTreatment>> GetByPersonAsync(
        Guid personId, CancellationToken cancellationToken)
        => await _context.ConcurrentTreatments
            .Where(c => c.PersonId == personId)
            .OrderBy(c => c.DayOfWeek)
            .ThenBy(c => c.StartTime)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<ConcurrentTreatment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.ConcurrentTreatments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(ConcurrentTreatment treatment, CancellationToken cancellationToken)
        => await _context.ConcurrentTreatments.AddAsync(treatment, cancellationToken);

    /// <inheritdoc />
    public Task RemoveAsync(ConcurrentTreatment treatment, CancellationToken cancellationToken)
    {
        _context.ConcurrentTreatments.Remove(treatment);
        return Task.CompletedTask;
    }
}
