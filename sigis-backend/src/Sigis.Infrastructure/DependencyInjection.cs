using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sigis.Domain.Interfaces;
using Sigis.Infrastructure.Persistence;
using Sigis.Infrastructure.Persistence.Context;
using Sigis.Infrastructure.Persistence.Interceptors;
using Sigis.Infrastructure.Persistence.Repositories;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure;

/// <summary>
/// Ponto único de registro dos serviços da camada de infraestrutura do
/// SIGIS (persistência, criptografia, relógio, usuário atual) no container
/// de injeção de dependência da aplicação (chamado a partir de
/// <c>Sigis.Api/Program.cs</c>).
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra o <see cref="SigisDbContext"/>, os 8 repositórios, a
    /// unidade de trabalho e os serviços de infraestrutura do SIGIS
    /// (criptografia, relógio, usuário atual).
    /// </summary>
    /// <param name="services">Coleção de serviços da aplicação.</param>
    /// <param name="configuration">
    /// Configuração da aplicação — deve conter <c>ConnectionStrings:SigisDb</c>
    /// e <c>Encryption:Key</c> (ou a variável de ambiente equivalente).
    /// </param>
    /// <returns>A própria <paramref name="services"/>, para encadeamento.</returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando a connection string <c>SigisDb</c> não está configurada.
    /// </exception>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Registra o MediatR aqui (e não só em Sigis.Application, no Prompt 4)
        // porque DomainEventDispatcherInterceptor já depende de IPublisher
        // nesta camada. Chamar AddMediatR novamente na Application, com o
        // assembly dela, é seguro e aditivo — cada camada registra os
        // handlers do seu próprio assembly.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IEncryptionService>(new AesEncryptionService(configuration));
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Autenticação (Parte B): hashing de senha e emissão de JWT.
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventDispatcherInterceptor>();

        var connectionString = configuration.GetConnectionString("SigisDb")
            ?? throw new InvalidOperationException("Connection string 'SigisDb' não configurada.");

        services.AddDbContext<SigisDbContext>((provider, options) =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(
                    provider.GetRequiredService<AuditInterceptor>(),
                    provider.GetRequiredService<DomainEventDispatcherInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IServiceUnitRepository, ServiceUnitRepository>();
        services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
        services.AddScoped<IQueueEntryRepository, QueueEntryRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IReferralRepository, ReferralRepository>();
        services.AddScoped<IDuplicateAlertRepository, DuplicateAlertRepository>();
        services.AddScoped<IAccessLogRepository, AccessLogRepository>();

        // Ajuste de cobertura: repositórios das 7 entidades novas.
        services.AddScoped<ISchoolHistoryRepository, SchoolHistoryRepository>();
        services.AddScoped<IFamilyCompositionRepository, FamilyCompositionRepository>();
        services.AddScoped<IDevelopmentMilestonesRepository, DevelopmentMilestonesRepository>();
        services.AddScoped<ILearningDifficultiesRepository, LearningDifficultiesRepository>();
        services.AddScoped<IPersonConsentRepository, PersonConsentRepository>();
        services.AddScoped<IPersonClinicalProfileRepository, PersonClinicalProfileRepository>();
        services.AddScoped<IConcurrentTreatmentRepository, ConcurrentTreatmentRepository>();

        // Autenticação (Parte B).
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
