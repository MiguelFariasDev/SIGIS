using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="DevelopmentMilestones"/> — marcos de
/// desenvolvimento motor, perceptivo e de linguagem (NAPE A.2 §5-6 e A.3
/// §6-7), 1:1 com <see cref="Person"/>.
/// </summary>
public sealed class DevelopmentMilestonesConfiguration : IEntityTypeConfiguration<DevelopmentMilestones>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DevelopmentMilestones> builder)
    {
        builder.ToTable("development_milestones");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.PersonId).HasColumnName("person_id").IsRequired();

        builder.Property(d => d.AgeWalkedMonths).HasColumnName("age_walked_months");
        builder.Property(d => d.AgeTalkedMonths).HasColumnName("age_talked_months");
        builder.Property(d => d.LocomotionDifficulty).HasColumnName("locomotion_difficulty");
        builder.Property(d => d.CoordinationDifficulty).HasColumnName("coordination_difficulty");
        builder.Property(d => d.VisualDifficulty).HasColumnName("visual_difficulty");
        builder.Property(d => d.HearingDifficulty).HasColumnName("hearing_difficulty");
        builder.Property(d => d.SpeechProblems).HasColumnName("speech_problems");
        builder.Property(d => d.CommandComprehension).HasColumnName("command_comprehension");
        builder.Property(d => d.CommunicationForm).HasColumnName("communication_form").HasMaxLength(100);
        builder.Property(d => d.ManualDominance).HasColumnName("manual_dominance").HasMaxLength(20);

        builder.Property(d => d.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne<Person>().WithMany().HasForeignKey(d => d.PersonId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.PersonId).HasDatabaseName("idx_development_milestones_person").IsUnique();
    }
}
