using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="SchoolHistory"/> — registro de uma
/// escola frequentada pela pessoa ao longo do tempo (NAPE A.3).
/// </summary>
public sealed class SchoolHistoryConfiguration : IEntityTypeConfiguration<SchoolHistory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SchoolHistory> builder)
    {
        builder.ToTable("school_history");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(s => s.SchoolName).HasColumnName("school_name").HasMaxLength(200).IsRequired();
        builder.Property(s => s.Grade).HasColumnName("grade").HasMaxLength(20).IsRequired();
        builder.Property(s => s.Shift).HasColumnName("shift").HasMaxLength(20);
        builder.Property(s => s.ClassGroup).HasColumnName("class_group").HasMaxLength(20);
        builder.Property(s => s.SchoolYear).HasColumnName("school_year").IsRequired();
        builder.Property(s => s.StartDate).HasColumnName("start_date").HasColumnType("date");
        builder.Property(s => s.EndDate).HasColumnName("end_date").HasColumnType("date");

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Notes).HasColumnName("notes");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.CreatedByProfessionalId).HasColumnName("created_by_professional_id").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(s => s.PersonId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.PersonId, s.SchoolYear }).HasDatabaseName("idx_school_history_person");

        // RN-SH01: no máximo um registro Ativo por pessoa.
        builder.HasIndex(s => s.PersonId)
            .HasDatabaseName("idx_school_history_active_unique")
            .IsUnique()
            .HasFilter("status = 'Ativo'");
    }
}
