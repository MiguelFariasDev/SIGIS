using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;
using Sigis.Infrastructure.Persistence.Converters;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="Professional"/> — profissional de
/// atendimento vinculado a uma unidade de serviço.
/// </summary>
public sealed class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.ToTable("professional");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.Name)
            .HasConversion(new PersonNameConverter())
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Specialty).HasColumnName("specialty").HasMaxLength(100).IsRequired();
        builder.Property(p => p.UnitId).HasColumnName("unit_id").IsRequired();

        builder.Property(p => p.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Active).HasColumnName("active").HasDefaultValue(true).IsRequired();

        // Autenticação — e-mail de login (único), hash de senha (BCrypt) e
        // último login. Nunca expor password_hash em DTOs.
        //
        // EmailAddressConverter é ValueConverter<EmailAddress?, string> (para
        // atender Person.Email, que é anulável); Professional.Email não é
        // anulável. Em tempo de execução ambos os tipos são idênticos — a
        // diferença é só de anotação de nulidade — por isso o aviso é
        // suprimido pontualmente aqui.
#pragma warning disable CS8620
        builder.Property(p => p.Email)
            .HasConversion(new EmailAddressConverter())
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired();
#pragma warning restore CS8620

        builder.Property(p => p.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(p => p.LastLoginAt).HasColumnName("last_login_at");

        builder.HasOne<ServiceUnit>().WithMany().HasForeignKey(p => p.UnitId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.UnitId).HasDatabaseName("idx_professional_unit");
        builder.HasIndex(p => p.Email).HasDatabaseName("idx_professional_email").IsUnique();
    }
}
