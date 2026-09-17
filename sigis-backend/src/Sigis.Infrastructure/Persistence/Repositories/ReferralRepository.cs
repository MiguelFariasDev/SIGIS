using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IReferralRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class ReferralRepository : IReferralRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="Referral"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public ReferralRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Referral?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Referrals.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Referral>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.Referrals
            .Where(r => r.PersonId == personId)
            .OrderByDescending(r => r.ReferralDate)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Referral>> GetActiveByDestinationUnitAsync(
        Guid destinationUnitId, CancellationToken cancellationToken)
        => await _context.Referrals
            .Where(r => r.DestinationUnitId == destinationUnitId
                && (r.Status == ReferralStatus.Pending || r.Status == ReferralStatus.Accepted))
            .OrderBy(r => r.ReferralDate)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Referral>> GetByOriginUnitAsync(Guid originUnitId, CancellationToken cancellationToken)
        => await _context.Referrals
            .Where(r => r.OriginUnitId == originUnitId)
            .OrderByDescending(r => r.ReferralDate)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(Referral referral, CancellationToken cancellationToken)
        => await _context.Referrals.AddAsync(referral, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Referral>> SearchAsync(
        Guid? unitId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        => await _context.Referrals
            .Where(r => (!unitId.HasValue || r.OriginUnitId == unitId.Value || r.DestinationUnitId == unitId.Value)
                && (!from.HasValue || r.ReferralDate >= from.Value)
                && (!to.HasValue || r.ReferralDate <= to.Value))
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(Referral referral, CancellationToken cancellationToken)
    {
        _context.Referrals.Update(referral);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> ReassignPersonAsync(Guid fromPersonId, Guid toPersonId, CancellationToken cancellationToken)
        => await _context.Referrals
            .Where(r => r.PersonId == fromPersonId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.PersonId, toPersonId), cancellationToken);
}
