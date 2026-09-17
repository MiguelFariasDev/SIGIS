using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;
using Sigis.Infrastructure.Persistence.Converters;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="Guardian"/> — responsável legal
/// vinculado a uma pessoa cadastrada.
/// </summary>
public sealed class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
{
    private readonly IEncryptionService _encryptionService;

    /// <summary>Cria a configuração de <see cref="Guardian"/>.</summary>
    /// <param name="encryptionService">Serviço usado para criptografar o CNS do responsável em repouso.</param>
    public GuardianConfiguration(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.ToTable("guardian");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasColumnName("id");

        builder.Property(g => g.PersonId).HasColumnName("person_id").IsRequired();

        builder.Property(g => g.Name)
            .HasConversion(new PersonNameConverter())
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(g => g.Cns)
            .HasConversion(new CnsConverter(_encryptionService))
            .HasColumnName("cns")
            .HasMaxLength(200);

        builder.Property(g => g.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date");

        builder.Property(g => g.Relationship)
            .HasColumnName("relationship")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(g => g.PersonId).HasDatabaseName("idx_guardian_person");
    }
}
