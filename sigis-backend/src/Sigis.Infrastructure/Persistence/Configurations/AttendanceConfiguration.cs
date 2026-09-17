using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="Attendance"/> — registro de uma
/// sessão de atendimento, com dados específicos do serviço em JSONB.
/// </summary>
public sealed class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("attendance");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(a => a.UnitId).HasColumnName("unit_id").IsRequired();
        builder.Property(a => a.ProfessionalId).HasColumnName("professional_id").IsRequired();
        builder.Property(a => a.DateTime).HasColumnName("date_time").IsRequired();

        builder.Property(a => a.SessionType)
            .HasColumnName("session_type")
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // dados_formulario: JSONB — cada serviço (NASF/NAPE) tem sua própria
        // estrutura de campos, sem exigir migração de schema a cada mudança.
        builder.Property(a => a.FormData)
            .HasColumnName("form_data")
            .HasColumnType("jsonb");

        builder.Property(a => a.SessionNumber).HasColumnName("session_number").IsRequired();

        // Triagem (NAPE A.2/A.3) — ajuste de cobertura.
        builder.Property(a => a.TriagedByProfessionalId).HasColumnName("triaged_by_professional_id");
        builder.Property(a => a.MainComplaint).HasColumnName("main_complaint").HasMaxLength(500);

        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(a => new { a.PersonId, a.DateTime }).HasDatabaseName("idx_attendance_person_date");
        builder.HasIndex(a => a.ProfessionalId).HasDatabaseName("idx_attendance_professional");
        builder.HasIndex(a => a.UnitId).HasDatabaseName("idx_attendance_unit");
        builder.HasIndex(a => a.TriagedByProfessionalId).HasDatabaseName("idx_attendance_triaged_by");
    }
}
