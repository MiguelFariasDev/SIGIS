using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IPersonRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL).
/// </summary>
public sealed class PersonRepository : IPersonRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de <see cref="Person"/>.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public PersonRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Persons
            .Include(p => p.Guardians)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<Person?> GetByCnsAsync(Cns cns, CancellationToken cancellationToken)
        => await _context.Persons
            .FirstOrDefaultAsync(p => p.Cns == cns, cancellationToken);

    /// <inheritdoc />
    public async Task<Person?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken)
        => await _context.Persons
            .FirstOrDefaultAsync(p => p.Cpf == cpf, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Person>> SearchByNameAsync(string term, CancellationToken cancellationToken)
    {
        // O termo de busca é normalizado pela mesma função (sigis.immutable_unaccent
        // + lower) usada para gerar name_normalized — necessário para que buscas
        // com ou sem acento ("José" ou "Jose") encontrem o mesmo resultado.
        const string sql = """
            SELECT p.* FROM sigis.person p
            WHERE p.name_normalized LIKE '%' || sigis.immutable_unaccent(lower(trim({0}))) || '%'
            ORDER BY p.name
            LIMIT 20
            """;

        return await _context.Persons
            .FromSqlRaw(sql, term)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Person>> FindDuplicateCandidatesAsync(
        PersonName name, DateOnly birthDate, CancellationToken cancellationToken)
    {
        // Camada 3 da deduplicação (RN02): matching probabilístico via
        // pg_trgm, com filtro duro por data de nascimento e limiar de
        // similaridade — ver DuplicateAlert.MinimumSimilarityThreshold.
        const string sql = """
            SELECT p.* FROM sigis.person p
            WHERE p.birth_date = {0}
              AND p.name_normalized % {1}
              AND similarity(p.name_normalized, {1}) >= 0.75
            ORDER BY similarity(p.name_normalized, {1}) DESC
            LIMIT 10
            """;

        return await _context.Persons
            .FromSqlRaw(sql, birthDate, name.Normalized)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Person person, CancellationToken cancellationToken)
        => await _context.Persons.AddAsync(person, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(Person person, CancellationToken cancellationToken)
    {
        _context.Persons.Update(person);
        return Task.CompletedTask;
    }
}
