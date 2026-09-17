using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IPersonClinicalProfileRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class PersonClinicalProfileRepository : IPersonClinicalProfileRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="PersonClinicalProfile"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public PersonClinicalProfileRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<PersonClinicalProfile?> GetByPersonAsync(Guid personId, CancellationToken cancellationToken)
        => await _context.PersonClinicalProfiles.FirstOrDefaultAsync(c => c.PersonId == personId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> ExistsMedicalRecordNumberInUnitAsync(
        Guid apsReferenceUnitId, string medicalRecordNumber, CancellationToken cancellationToken)
        => await _context.PersonClinicalProfiles.AnyAsync(
            c => c.ApsReferenceUnitId == apsReferenceUnitId && c.MedicalRecordNumber == medicalRecordNumber,
            cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(PersonClinicalProfile profile, CancellationToken cancellationToken)
        => await _context.PersonClinicalProfiles.AddAsync(profile, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(PersonClinicalProfile profile, CancellationToken cancellationToken)
    {
        _context.PersonClinicalProfiles.Update(profile);
        return Task.CompletedTask;
    }
}
