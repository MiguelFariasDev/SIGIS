using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IAccessLogRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class AccessLogRepository : IAccessLogRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="AccessLog"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public AccessLogRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AccessLog>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.AccessLogs
            .Where(l => l.PersonId == personId)
            .OrderByDescending(l => l.DateTime)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<AccessLog>> GetByProfessionalAsync(
        Guid professionalId, CancellationToken cancellationToken)
        => await _context.AccessLogs
            .Where(l => l.ProfessionalId == professionalId)
            .OrderByDescending(l => l.DateTime)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(AccessLog accessLog, CancellationToken cancellationToken)
        => await _context.AccessLogs.AddAsync(accessLog, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<AccessLog>> SearchAsync(
        Guid? personId, Guid? professionalId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        => await _context.AccessLogs
            .Where(l => (!personId.HasValue || l.PersonId == personId.Value)
                && (!professionalId.HasValue || l.ProfessionalId == professionalId.Value)
                && (!from.HasValue || l.DateTime >= from.Value)
                && (!to.HasValue || l.DateTime <= to.Value))
            .OrderByDescending(l => l.DateTime)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<AccessLog>> GetCrossUnitAsync(CancellationToken cancellationToken)
        => await _context.AccessLogs
            .Where(l => EF.Property<bool>(l, "_isCrossUnit"))
            .OrderByDescending(l => l.DateTime)
            .ToListAsync(cancellationToken);
}
