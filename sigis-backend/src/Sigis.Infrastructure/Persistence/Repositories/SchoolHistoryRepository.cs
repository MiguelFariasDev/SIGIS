using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="ISchoolHistoryRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class SchoolHistoryRepository : ISchoolHistoryRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="SchoolHistory"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public SchoolHistoryRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<SchoolHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.SchoolHistories.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<SchoolHistory>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.SchoolHistories
            .Where(s => s.PersonId == personId)
            .OrderByDescending(s => s.SchoolYear)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<SchoolHistory?> GetActiveByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.SchoolHistories
            .FirstOrDefaultAsync(s => s.PersonId == personId && s.Status == SchoolStatus.Ativo, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(SchoolHistory schoolHistory, CancellationToken cancellationToken)
        => await _context.SchoolHistories.AddAsync(schoolHistory, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(SchoolHistory schoolHistory, CancellationToken cancellationToken)
    {
        _context.SchoolHistories.Update(schoolHistory);
        return Task.CompletedTask;
    }
}
