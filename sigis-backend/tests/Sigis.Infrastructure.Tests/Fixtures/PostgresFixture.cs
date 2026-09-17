using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sigis.Infrastructure.Persistence.Context;
using Sigis.Infrastructure.Services;
using Testcontainers.PostgreSql;

namespace Sigis.Infrastructure.Tests.Fixtures;

/// <summary>
/// Fixture de testes de integração que sobe um container PostgreSQL 16 real
/// via Testcontainers, aplica as migrations do SIGIS (incluindo extensões
/// <c>pg_trgm</c>/<c>unaccent</c>/<c>pgcrypto</c> e índices especiais) e
/// expõe instâncias de <see cref="SigisDbContext"/> prontas para uso — sem
/// mocks, validando o comportamento real do banco.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;
    private IEncryptionService _encryptionService = null!;

    /// <summary>Sobe o container PostgreSQL e aplica as migrations do SIGIS.</summary>
    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("sigis_db")
            .WithUsername("sigis")
            .WithPassword("sigis_dev")
            .Build();

        await _container.StartAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:Key"] = "integration-test-key-not-for-production",
            })
            .Build();

        _encryptionService = new AesEncryptionService(configuration);

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Cria uma nova instância de <see cref="SigisDbContext"/> conectada ao
    /// container de testes — uma por chamada, para que cada teste tenha seu
    /// próprio <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker"/>.
    /// </summary>
    /// <returns>Um <see cref="SigisDbContext"/> pronto para uso.</returns>
    public SigisDbContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SigisDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .UseSnakeCaseNamingConvention();

        return new SigisDbContext(optionsBuilder.Options, _encryptionService);
    }

    /// <summary>Encerra e remove o container PostgreSQL de testes.</summary>
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

/// <summary>
/// Agrupa todas as classes de teste de integração sob uma única instância de
/// <see cref="PostgresFixture"/> (um único container para toda a suíte,
/// evitando o custo de subir um container por classe de teste).
/// </summary>
[CollectionDefinition("Postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>;
