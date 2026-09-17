using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IDevelopmentMilestonesRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class DevelopmentMilestonesRepository : IDevelopmentMilestonesRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="DevelopmentMilestones"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public DevelopmentMilestonesRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<DevelopmentMilestones?> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.DevelopmentMilestones.FirstOrDefaultAsync(d => d.PersonId == personId, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(DevelopmentMilestones milestones, CancellationToken cancellationToken)
        => await _context.DevelopmentMilestones.AddAsync(milestones, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(DevelopmentMilestones milestones, CancellationToken cancellationToken)
    {
        _context.DevelopmentMilestones.Update(milestones);
        return Task.CompletedTask;
    }
}
