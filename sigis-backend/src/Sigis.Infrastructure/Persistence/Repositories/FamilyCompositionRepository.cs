using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IFamilyCompositionRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class FamilyCompositionRepository : IFamilyCompositionRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="FamilyComposition"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public FamilyCompositionRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<FamilyComposition?> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.FamilyCompositions.FirstOrDefaultAsync(f => f.PersonId == personId, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(FamilyComposition familyComposition, CancellationToken cancellationToken)
        => await _context.FamilyCompositions.AddAsync(familyComposition, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(FamilyComposition familyComposition, CancellationToken cancellationToken)
    {
        _context.FamilyCompositions.Update(familyComposition);
        return Task.CompletedTask;
    }
}
