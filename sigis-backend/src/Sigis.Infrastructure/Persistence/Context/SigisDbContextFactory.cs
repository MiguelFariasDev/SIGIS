using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure.Persistence.Context;

/// <summary>
/// Fábrica usada em tempo de design pelas ferramentas de linha de comando do
/// EF Core (<c>dotnet ef migrations add</c>, <c>dotnet ef database
/// update</c>) para criar uma instância de <see cref="SigisDbContext"/> fora
/// do pipeline de injeção de dependência da aplicação (o projeto
/// <c>Sigis.Api</c> nem precisa estar em execução).
/// </summary>
public sealed class SigisDbContextFactory : IDesignTimeDbContextFactory<SigisDbContext>
{
    /// <summary>
    /// Cria o <see cref="SigisDbContext"/> em tempo de design, lendo a
    /// connection string e a chave de criptografia do <c>appsettings</c> do
    /// projeto de inicialização (<c>Sigis.Api</c>) ou de variáveis de
    /// ambiente, com um valor padrão de desenvolvimento como último recurso.
    /// </summary>
    /// <param name="args">Argumentos de linha de comando repassados pela ferramenta do EF Core.</param>
    /// <returns>Uma instância pronta para gerar/aplicar migrations.</returns>
    public SigisDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("SigisDb")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__SigisDb")
            ?? "Host=localhost;Port=5432;Database=sigis_db;Username=sigis;Password=sigis_dev";

        var optionsBuilder = new DbContextOptionsBuilder<SigisDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new SigisDbContext(optionsBuilder.Options, new AesEncryptionService(configuration));
    }
}
