using Sigis.Domain.Interfaces;

namespace Sigis.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="IDateTimeProvider"/> baseada no relógio real
/// do sistema (<see cref="DateTime.UtcNow"/>).
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc />
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
