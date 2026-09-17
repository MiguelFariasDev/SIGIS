using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="ILearningDifficultiesRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class LearningDifficultiesRepository : ILearningDifficultiesRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="LearningDifficulties"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public LearningDifficultiesRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LearningDifficulties>> GetByPersonAsync(
        Guid personId, CancellationToken cancellationToken)
        => await _context.LearningDifficulties
            .Where(l => l.PersonId == personId)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<LearningDifficulties?> GetByPersonAndTypeAsync(
        Guid personId, LearningDifficultyType type, CancellationToken cancellationToken)
        => await _context.LearningDifficulties
            .FirstOrDefaultAsync(l => l.PersonId == personId && l.Type == type, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(LearningDifficulties learningDifficulty, CancellationToken cancellationToken)
        => await _context.LearningDifficulties.AddAsync(learningDifficulty, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(LearningDifficulties learningDifficulty, CancellationToken cancellationToken)
    {
        _context.LearningDifficulties.Update(learningDifficulty);
        return Task.CompletedTask;
    }
}
