using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="FamilyComposition"/> — composição
/// familiar de uma pessoa (NAPE A.2 §2 e A.3 §2-3), 1:1 com <see cref="Person"/>.
/// </summary>
public sealed class FamilyCompositionConfiguration : IEntityTypeConfiguration<FamilyComposition>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FamilyComposition> builder)
    {
        builder.ToTable("family_composition");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id");

        builder.Property(f => f.PersonId).HasColumnName("person_id").IsRequired();

        builder.Property(f => f.FatherName).HasColumnName("father_name").HasMaxLength(200);
        builder.Property(f => f.FatherEducation).HasColumnName("father_education").HasMaxLength(50);
        builder.Property(f => f.FatherOccupation).HasColumnName("father_occupation").HasMaxLength(100);
        builder.Property(f => f.MotherName).HasColumnName("mother_name").HasMaxLength(200);
        builder.Property(f => f.MotherEducation).HasColumnName("mother_education").HasMaxLength(50);
        builder.Property(f => f.MotherOccupation).HasColumnName("mother_occupation").HasMaxLength(100);

        builder.Property(f => f.SiblingsCount).HasColumnName("siblings_count");
        builder.Property(f => f.SiblingsAges).HasColumnName("siblings_ages").HasMaxLength(100);
        builder.Property(f => f.HouseholdMembersCount).HasColumnName("household_members_count");
        builder.Property(f => f.ParentsMaritalStatus).HasColumnName("parents_marital_status").HasMaxLength(50);

        builder.Property(f => f.FiliationType).HasColumnName("filiation_type").HasMaxLength(20);
        builder.Property(f => f.PlannedPregnancy).HasColumnName("planned_pregnancy");
        builder.Property(f => f.PregnanciesCount).HasColumnName("pregnancies_count");
        builder.Property(f => f.AbortionsCount).HasColumnName("abortions_count");
        builder.Property(f => f.PregnancyHealthIssue).HasColumnName("pregnancy_health_issue");
        builder.Property(f => f.DeliveryType).HasColumnName("delivery_type").HasMaxLength(20);
        builder.Property(f => f.MedicationDuringPregnancy).HasColumnName("medication_during_pregnancy");

        builder.Property(f => f.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(f => f.PersonId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => f.PersonId).HasDatabaseName("idx_family_composition_person").IsUnique();
    }
}
