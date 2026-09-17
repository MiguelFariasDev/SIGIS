using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="QueueEntry"/> — entrada de uma pessoa
/// na fila de atendimento de uma unidade de serviço.
/// </summary>
public sealed class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<QueueEntry> builder)
    {
        builder.ToTable("queue_entry");

        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).HasColumnName("id");

        builder.Property(q => q.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(q => q.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(q => q.Specialty).HasColumnName("specialty").HasMaxLength(100).IsRequired();

        builder.Property(q => q.Priority)
            .HasColumnName("priority")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(q => q.EnteredAt).HasColumnName("entered_at").IsRequired();

        builder.Property(q => q.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(q => q.ConsecutiveAbsences)
            .HasColumnName("consecutive_absences")
            .HasDefaultValue(0)
            .IsRequired();

        // RN01: no máximo uma entrada ATIVA (aguardando/em atendimento) por
        // pessoa e unidade — faltas/busca ativa/conclusão liberam a unicidade.
        builder.HasIndex(q => new { q.PersonId, q.UnitId })
            .HasDatabaseName("idx_queue_active_unique")
            .IsUnique()
            .HasFilter("status IN ('Waiting', 'InAttendance')");

        builder.HasIndex(q => new { q.UnitId, q.Status })
            .HasDatabaseName("idx_queue_unit_status");
    }
}
