using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sigis.Domain.Interfaces;

namespace Sigis.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor de <see cref="DbContext.SaveChanges"/> que atualiza
/// automaticamente <see cref="IAuditable.UpdatedAt"/> em agregados
/// modificados, usando <see cref="IDateTimeProvider"/> em vez do relógio do
/// sistema diretamente — mantém a auditoria testável e centralizada, sem que
/// cada caso de uso precise lembrar de tocar o carimbo de tempo manualmente.
/// </summary>
/// <remarks>
/// A criação (<c>CreatedAt</c>) já é responsabilidade do próprio construtor
/// de domínio de cada agregado — só <see cref="IAuditable.UpdatedAt"/> é
/// tratado aqui, e apenas para agregados que implementam a interface (ver
/// <c>IAuditable</c> em <c>Sigis.Domain</c>).
/// </remarks>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>Cria o interceptor de auditoria.</summary>
    /// <param name="dateTimeProvider">Provedor de data/hora usado para o carimbo de atualização.</param>
    public AuditInterceptor(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
            return;

        var now = _dateTimeProvider.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Modified)
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = now;
        }
    }
}
