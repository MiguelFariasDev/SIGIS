using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="ConcurrentTreatment"/> — atendimento
/// concomitante que a pessoa recebe em outro serviço (NAPE A.4).
/// </summary>
public sealed class ConcurrentTreatmentConfiguration : IEntityTypeConfiguration<ConcurrentTreatment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ConcurrentTreatment> builder)
    {
        builder.ToTable("concurrent_treatment");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(c => c.Specialty).HasColumnName("specialty").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Location).HasColumnName("location").HasMaxLength(150).IsRequired();
        builder.Property(c => c.ProfessionalName).HasColumnName("professional_name").HasMaxLength(200).IsRequired();

        builder.Property(c => c.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.StartTime).HasColumnName("start_time").HasColumnType("time").IsRequired();
        builder.Property(c => c.EndTime).HasColumnName("end_time").HasColumnType("time").IsRequired();
        builder.Property(c => c.Notes).HasColumnName("notes");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(c => c.PersonId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.PersonId, c.DayOfWeek }).HasDatabaseName("idx_concurrent_treatment_person");
    }
}
