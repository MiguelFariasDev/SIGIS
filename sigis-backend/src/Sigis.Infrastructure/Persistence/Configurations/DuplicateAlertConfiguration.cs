using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="DuplicateAlert"/> — alerta de
/// possível duplicidade de cadastro entre duas pessoas.
/// </summary>
public sealed class DuplicateAlertConfiguration : IEntityTypeConfiguration<DuplicateAlert>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DuplicateAlert> builder)
    {
        builder.ToTable("duplicate_alert");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.PersonId1).HasColumnName("person_id_1").IsRequired();
        builder.Property(d => d.PersonId2).HasColumnName("person_id_2").IsRequired();

        // double precision — nunca float (perda de precisão em comparações
        // de limiar como 0.75, ver claude.md "Regras de ouro").
        builder.Property(d => d.SimilarityScore).HasColumnName("similarity_score").IsRequired();

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.ResolvedByProfessionalId).HasColumnName("resolved_by_professional_id");
        builder.Property(d => d.ResolvedAt).HasColumnName("resolved_at");
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(d => d.Status).HasDatabaseName("idx_duplicate_alert_status");

        // Nota: unicidade por par ordenado (person_id_1, person_id_2) — não
        // impede um par inverso (person_id_2, person_id_1) de ser criado
        // separadamente. Normalizar a ordem do par é responsabilidade da
        // camada de aplicação (Prompt 4), que decide qual id vai em qual campo.
        builder.HasIndex(d => new { d.PersonId1, d.PersonId2 })
            .IsUnique()
            .HasDatabaseName("idx_duplicate_alert_pair");
    }
}
