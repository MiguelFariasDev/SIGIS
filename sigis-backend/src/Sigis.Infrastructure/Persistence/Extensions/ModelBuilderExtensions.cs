using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sigis.Infrastructure.Persistence.Extensions;

/// <summary>
/// Extensões para <see cref="ModelBuilder"/> usadas na configuração global
/// do modelo de dados do SIGIS.
/// </summary>
public static class ModelBuilderExtensions
{
    private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter = new(
        toProvider => toProvider.ToUniversalTime(),
        fromProvider => DateTime.SpecifyKind(fromProvider, DateTimeKind.Utc));

    /// <summary>
    /// Garante que toda propriedade <see cref="DateTime"/> do modelo seja
    /// lida de volta do PostgreSQL com <see cref="DateTimeKind.Utc"/>.
    /// </summary>
    /// <remarks>
    /// O provedor Npgsql retorna colunas <c>timestamp</c> como
    /// <see cref="DateTimeKind.Unspecified"/> por padrão. Isso não afeta
    /// regras de negócio (todo o domínio já trabalha em UTC — ver convenção
    /// "usar DateTime.UtcNow" do <c>claude.md</c>), mas quebraria a
    /// serialização JSON das respostas da API (Prompt 4): um
    /// <see cref="DateTime"/> com <c>Kind=Unspecified</c> é serializado pelo
    /// <c>System.Text.Json</c> sem o sufixo "Z", levando o frontend a
    /// interpretá-lo incorretamente como horário local do navegador.
    /// </remarks>
    /// <param name="modelBuilder">Model builder do <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.</param>
    /// <returns>O próprio <paramref name="modelBuilder"/>, para encadeamento.</returns>
    public static ModelBuilder EnforceUtcDateTimes(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var dateTimeProperties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in dateTimeProperties)
                property.SetValueConverter(UtcDateTimeConverter);
        }

        return modelBuilder;
    }
}
