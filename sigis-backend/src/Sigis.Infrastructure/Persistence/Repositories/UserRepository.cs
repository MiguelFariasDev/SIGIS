using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Persistence.Context;

namespace Sigis.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação de <see cref="IUserRepository"/> sobre o
/// <see cref="SigisDbContext"/> (PostgreSQL) — acesso a <see cref="Professional"/>
/// voltado à autenticação.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly SigisDbContext _context;

    /// <summary>Cria o repositório de autenticação.</summary>
    /// <param name="context">Contexto de persistência do SIGIS.</param>
    public UserRepository(SigisDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Professional?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken)
        => await _context.Professionals.FirstOrDefaultAsync(p => p.Email == email, cancellationToken);

    /// <inheritdoc />
    public async Task<Professional?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.Professionals.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(Professional professional, CancellationToken cancellationToken)
    {
        _context.Professionals.Update(professional);
        return Task.CompletedTask;
    }
}
