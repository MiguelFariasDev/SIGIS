using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="Referral"/> — encaminhamento de uma
/// pessoa entre unidades de serviço da rede.
/// </summary>
public sealed class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.ToTable("referral");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(r => r.OriginUnitId).HasColumnName("origin_unit_id").IsRequired();
        builder.Property(r => r.DestinationUnitId).HasColumnName("destination_unit_id").IsRequired();
        builder.Property(r => r.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();

        builder.Property(r => r.Priority)
            .HasColumnName("priority")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.ReferralDate).HasColumnName("referral_date").IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.CorrelationId).HasColumnName("correlation_id").IsRequired();

        builder.HasIndex(r => r.CorrelationId).IsUnique().HasDatabaseName("idx_referral_correlation");
        builder.HasIndex(r => new { r.DestinationUnitId, r.Status }).HasDatabaseName("idx_referral_destination_status");
        builder.HasIndex(r => r.PersonId).HasDatabaseName("idx_referral_person");
    }
}
