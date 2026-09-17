using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IAttendanceRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class AttendanceRepository : IAttendanceRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="Attendance"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public AttendanceRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Attendances.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Attendance>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.Attendances
            .Where(a => a.PersonId == personId)
            .OrderByDescending(a => a.DateTime)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<bool> HasActiveAttendanceInOtherUnitAsync(
        Guid personId, Guid excludingUnitId, CancellationToken cancellationToken)
        => await _context.Attendances.AnyAsync(
            a => a.PersonId == personId
                && a.UnitId != excludingUnitId
                && a.Status == AttendanceStatus.Scheduled,
            cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(Attendance attendance, CancellationToken cancellationToken)
        => await _context.Attendances.AddAsync(attendance, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken)
    {
        _context.Attendances.Update(attendance);
        return Task.CompletedTask;
    }
}
