using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="AccessLog"/> — registro de auditoria
/// de acesso a dados de uma pessoa, exigido pela LGPD (art. 11, II).
/// </summary>
public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("access_log");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");

        builder.Property(l => l.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(l => l.ProfessionalId).HasColumnName("professional_id").IsRequired();
        builder.Property(l => l.Action).HasColumnName("action").HasMaxLength(30).IsRequired();
        builder.Property(l => l.LegalBasis).HasColumnName("legal_basis").HasMaxLength(50).IsRequired();
        builder.Property(l => l.Justification).HasColumnName("justification").HasMaxLength(500);
        builder.Property(l => l.DateTime).HasColumnName("date_time").IsRequired();
        builder.Property(l => l.Ip).HasColumnName("ip").HasMaxLength(45);
        builder.Property(l => l.UserAgent).HasColumnName("user_agent").HasMaxLength(300);

        // IsCrossUnit() é campo privado no domínio (calculado no momento da
        // criação a partir do que a Application informa) — persistido via
        // shadow property, sem expor setter público na entidade de domínio.
        builder.Property<bool>("_isCrossUnit")
            .HasColumnName("is_cross_unit")
            .IsRequired();

        builder.HasIndex(l => l.ProfessionalId).HasDatabaseName("idx_access_log_professional");
        builder.HasIndex(l => l.PersonId).HasDatabaseName("idx_access_log_person");
        builder.HasIndex(l => l.DateTime).HasDatabaseName("idx_access_log_date");
    }
}
