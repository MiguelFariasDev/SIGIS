using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IProfessionalRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class ProfessionalRepository : IProfessionalRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="Professional"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public ProfessionalRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Professional?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Professional>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken)
        => await _context.Professionals
            .Where(p => p.UnitId == unitId)
            .OrderBy(p => p.Name.Value)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(Professional professional, CancellationToken cancellationToken)
        => await _context.Professionals.AddAsync(professional, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(Professional professional, CancellationToken cancellationToken)
    {
        _context.Professionals.Update(professional);
        return Task.CompletedTask;
    }
}
