using Microsoft.EntityFrameworkCore;
using Sigis.Domain.Entities;
using Sigis.Infrastructure.Persistence.Configurations;
using Sigis.Infrastructure.Persistence.Extensions;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure.Persistence.Context;

/// <summary>
/// Contexto de persistência do SIGIS — ponto único de acesso ao banco
/// PostgreSQL via Entity Framework Core, com todas as tabelas mapeadas no
/// schema <c>sigis</c>.
/// </summary>
public sealed class SigisDbContext : DbContext
{
    private readonly IEncryptionService _encryptionService;

    /// <summary>Pessoas cadastradas — núcleo de identidade canônica da rede.</summary>
    public DbSet<Person> Persons => Set<Person>();

    /// <summary>Responsáveis legais vinculados a pessoas.</summary>
    public DbSet<Guardian> Guardians => Set<Guardian>();

    /// <summary>Unidades de serviço da rede municipal (NASF, NAPE, CREAES, etc.).</summary>
    public DbSet<ServiceUnit> ServiceUnits => Set<ServiceUnit>();

    /// <summary>Profissionais de atendimento vinculados a unidades.</summary>
    public DbSet<Professional> Professionals => Set<Professional>();

    /// <summary>Entradas na fila de atendimento das unidades.</summary>
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();

    /// <summary>Registros de atendimento/sessão realizados ou agendados.</summary>
    public DbSet<Attendance> Attendances => Set<Attendance>();

    /// <summary>Encaminhamentos de pessoas entre unidades da rede.</summary>
    public DbSet<Referral> Referrals => Set<Referral>();

    /// <summary>Alertas de possível duplicidade de cadastro entre pessoas.</summary>
    public DbSet<DuplicateAlert> DuplicateAlerts => Set<DuplicateAlert>();

    /// <summary>Registros de auditoria de acesso a dados de pacientes (LGPD).</summary>
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    /// <summary>Perfis clínicos NASF (ficha A.1) — ajuste de cobertura.</summary>
    public DbSet<PersonClinicalProfile> PersonClinicalProfiles => Set<PersonClinicalProfile>();

    /// <summary>Atendimentos concomitantes em outros serviços (NAPE A.4) — ajuste de cobertura.</summary>
    public DbSet<ConcurrentTreatment> ConcurrentTreatments => Set<ConcurrentTreatment>();

    /// <summary>Histórico de escolas frequentadas (NAPE A.3) — ajuste de cobertura.</summary>
    public DbSet<SchoolHistory> SchoolHistories => Set<SchoolHistory>();

    /// <summary>Composição familiar (NAPE A.2/A.3) — ajuste de cobertura.</summary>
    public DbSet<FamilyComposition> FamilyCompositions => Set<FamilyComposition>();

    /// <summary>Marcos de desenvolvimento (NAPE A.2/A.3) — ajuste de cobertura.</summary>
    public DbSet<DevelopmentMilestones> DevelopmentMilestones => Set<DevelopmentMilestones>();

    /// <summary>Dificuldades de aprendizagem identificadas na sondagem (NAPE A.3) — ajuste de cobertura.</summary>
    public DbSet<LearningDifficulties> LearningDifficulties => Set<LearningDifficulties>();

    /// <summary>Consentimentos granulares LGPD concedidos por pessoas — ajuste de cobertura.</summary>
    public DbSet<PersonConsent> PersonConsents => Set<PersonConsent>();

    /// <summary>
    /// Cria o contexto de persistência do SIGIS.
    /// </summary>
    /// <param name="options">Opções de configuração do EF Core (connection string, provider, interceptors).</param>
    /// <param name="encryptionService">
    /// Serviço de criptografia usado pelos conversores de CNS/CPF. É recebido
    /// aqui — em vez de instanciado dentro de cada configuração de entidade —
    /// porque <c>PersonConfiguration</c> e <c>GuardianConfiguration</c>
    /// precisam dele no construtor, o que exige aplicá-las explicitamente em
    /// <see cref="OnModelCreating"/> (em vez de via
    /// <c>ApplyConfigurationsFromAssembly</c>, que só funciona com tipos de
    /// construtor sem parâmetros).
    /// </param>
    public SigisDbContext(DbContextOptions<SigisDbContext> options, IEncryptionService encryptionService)
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("sigis");

        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.HasPostgresExtension("unaccent");
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.ApplyConfiguration(new PersonConfiguration(_encryptionService));
        modelBuilder.ApplyConfiguration(new GuardianConfiguration(_encryptionService));
        modelBuilder.ApplyConfiguration(new ServiceUnitConfiguration());
        modelBuilder.ApplyConfiguration(new ProfessionalConfiguration());
        modelBuilder.ApplyConfiguration(new QueueEntryConfiguration());
        modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
        modelBuilder.ApplyConfiguration(new ReferralConfiguration());
        modelBuilder.ApplyConfiguration(new DuplicateAlertConfiguration());
        modelBuilder.ApplyConfiguration(new AccessLogConfiguration());
        modelBuilder.ApplyConfiguration(new PersonClinicalProfileConfiguration());
        modelBuilder.ApplyConfiguration(new ConcurrentTreatmentConfiguration());
        modelBuilder.ApplyConfiguration(new SchoolHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new FamilyCompositionConfiguration());
        modelBuilder.ApplyConfiguration(new DevelopmentMilestonesConfiguration());
        modelBuilder.ApplyConfiguration(new LearningDifficultiesConfiguration());
        modelBuilder.ApplyConfiguration(new PersonConsentConfiguration());

        modelBuilder.EnforceUtcDateTimes();

        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().HaveMaxLength(500);
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);

        base.ConfigureConventions(configurationBuilder);
    }
}
