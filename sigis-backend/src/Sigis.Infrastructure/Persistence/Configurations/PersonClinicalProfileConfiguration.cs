using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="PersonClinicalProfile"/> — perfil
/// clínico NASF (ficha A.1), 1:1 com <see cref="Person"/>.
/// </summary>
public sealed class PersonClinicalProfileConfiguration : IEntityTypeConfiguration<PersonClinicalProfile>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PersonClinicalProfile> builder)
    {
        builder.ToTable("person_clinical_profile");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(c => c.MedicalRecordNumber).HasColumnName("medical_record_number").HasMaxLength(50);
        builder.Property(c => c.ClinicalHypothesis).HasColumnName("clinical_hypothesis");
        builder.Property(c => c.ApsReferenceUnitId).HasColumnName("aps_reference_unit_id");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(c => c.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ServiceUnit>().WithMany()
            .HasForeignKey(c => c.ApsReferenceUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PersonId).HasDatabaseName("idx_person_clinical_profile_person").IsUnique();

        // RN-CP02: número de prontuário único dentro da mesma unidade APS.
        builder.HasIndex(c => new { c.ApsReferenceUnitId, c.MedicalRecordNumber })
            .HasDatabaseName("idx_person_clinical_profile_record_unique")
            .IsUnique()
            .HasFilter("medical_record_number IS NOT NULL");
    }
}
