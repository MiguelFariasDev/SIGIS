using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="PersonConsent"/> — consentimento
/// granular LGPD concedido por uma pessoa (LGPD, art. 8º e art. 11, I).
/// </summary>
public sealed class PersonConsentConfiguration : IEntityTypeConfiguration<PersonConsent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PersonConsent> builder)
    {
        builder.ToTable("person_consent");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.PersonId).HasColumnName("person_id").IsRequired();

        builder.Property(c => c.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Granted).HasColumnName("granted").IsRequired();
        builder.Property(c => c.GrantedAt).HasColumnName("granted_at").IsRequired();
        builder.Property(c => c.GrantedByGuardianId).HasColumnName("granted_by_guardian_id");
        builder.Property(c => c.RevokedAt).HasColumnName("revoked_at");
        builder.Property(c => c.Version).HasColumnName("version").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Evidence).HasColumnName("evidence").HasMaxLength(200);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(c => c.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Guardian>().WithMany().HasForeignKey(c => c.GrantedByGuardianId).OnDelete(DeleteBehavior.Restrict);

        // RN-PC01: só um consentimento ativo (não revogado) por tipo e pessoa.
        builder.HasIndex(c => new { c.PersonId, c.Type })
            .HasDatabaseName("idx_person_consent_active_unique")
            .IsUnique()
            .HasFilter("revoked_at IS NULL");

        builder.HasIndex(c => c.PersonId).HasDatabaseName("idx_person_consent_person");
    }
}
