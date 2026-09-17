using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="LearningDifficulties"/> — dificuldade
/// de aprendizagem identificada na sondagem psicopedagógica (NAPE A.3 §5).
/// </summary>
public sealed class LearningDifficultiesConfiguration : IEntityTypeConfiguration<LearningDifficulties>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LearningDifficulties> builder)
    {
        builder.ToTable("learning_difficulties");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");

        builder.Property(l => l.PersonId).HasColumnName("person_id").IsRequired();

        builder.Property(l => l.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(l => l.Severity).HasColumnName("severity").HasMaxLength(20);

        // Data da sondagem psicopedagógica (NAPE A.3 §1) — lacuna 5 do ajuste de cobertura.
        builder.Property(l => l.AssessmentDate).HasColumnName("assessment_date").HasColumnType("date");

        builder.Property(l => l.Notes).HasColumnName("notes");
        builder.Property(l => l.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(l => l.PersonId).OnDelete(DeleteBehavior.Cascade);

        // RN-LD01: uma pessoa não pode ter 2 registros do mesmo tipo.
        builder.HasIndex(l => new { l.PersonId, l.Type })
            .HasDatabaseName("idx_learning_difficulties_person_type")
            .IsUnique();

        builder.HasIndex(l => l.PersonId).HasDatabaseName("idx_learning_difficulties_person");
    }
}
