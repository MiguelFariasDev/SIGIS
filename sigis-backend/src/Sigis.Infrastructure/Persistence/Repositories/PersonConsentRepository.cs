using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IPersonConsentRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class PersonConsentRepository : IPersonConsentRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="PersonConsent"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public PersonConsentRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<PersonConsent?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.PersonConsents.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<PersonConsent>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.PersonConsents
            .Where(c => c.PersonId == personId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<PersonConsent?> GetActiveByPersonAndTypeAsync(
        Guid personId, ConsentType type, CancellationToken cancellationToken)
        => await _context.PersonConsents
            .FirstOrDefaultAsync(c => c.PersonId == personId && c.Type == type && c.RevokedAt == null, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(PersonConsent consent, CancellationToken cancellationToken)
        => await _context.PersonConsents.AddAsync(consent, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(PersonConsent consent, CancellationToken cancellationToken)
    {
        _context.PersonConsents.Update(consent);
        return Task.CompletedTask;
    }
}
