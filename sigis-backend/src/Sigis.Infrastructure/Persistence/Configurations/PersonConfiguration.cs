using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;
using Sigis.Infrastructure.Persistence.Converters;
using Sigis.Infrastructure.Services;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional do agregado <see cref="Person"/> — núcleo de
/// identidade canônica da pessoa, compartilhado por toda a rede de serviços.
/// </summary>
public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    private readonly IEncryptionService _encryptionService;

    /// <summary>Cria a configuração de <see cref="Person"/>.</summary>
    /// <param name="encryptionService">Serviço usado para criptografar CNS e CPF em repouso.</param>
    public PersonConfiguration(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("person");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.Name)
            .HasConversion(new PersonNameConverter())
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        // Coluna gerada (armazenada) para busca fuzzy via pg_trgm — nunca
        // escrita pela aplicação, apenas lida via EF.Property<string>.
        //
        // Usa sigis.immutable_unaccent(...) em vez de unaccent(...) direto:
        // a função unaccent() da extensão é marcada STABLE (depende de
        // configuração de dicionário), e o PostgreSQL exige que a expressão
        // de uma coluna GENERATED seja IMMUTABLE — a função wrapper é criada
        // via SQL bruto na migration inicial (ver InitialCreate.Up).
        builder.Property<string>("name_normalized")
            .HasColumnName("name_normalized")
            .HasColumnType("text")
            .HasComputedColumnSql("sigis.immutable_unaccent(lower(name))", stored: true);

        builder.Property(p => p.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(p => p.Cns)
            .HasConversion(new CnsConverter(_encryptionService))
            .HasColumnName("cns")
            .HasMaxLength(200);

        builder.Property(p => p.Cpf)
            .HasConversion(new CpfConverter(_encryptionService))
            .HasColumnName("cpf")
            .HasMaxLength(200);

        builder.Property(p => p.MotherName)
            .HasColumnName("mother_name")
            .HasMaxLength(150);

        builder.Property(p => p.Gender)
            .HasColumnName("gender")
            .HasMaxLength(20);

        builder.Property(p => p.RaceColor)
            .HasColumnName("race_color")
            .HasMaxLength(30);

        builder.Property(p => p.Phone)
            .HasConversion(new PhoneNumberConverter())
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasConversion(new EmailAddressConverter())
            .HasColumnName("email")
            .HasMaxLength(254);

        // Address é owned entity — colunas ficam direto em "person"
        builder.OwnsOne(p => p.Address, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("street").HasMaxLength(200);
            addr.Property(a => a.Number).HasColumnName("number").HasMaxLength(20);
            addr.Property(a => a.Neighborhood).HasColumnName("neighborhood").HasMaxLength(100);
            addr.Property(a => a.City).HasColumnName("city").HasMaxLength(100);
            addr.Property(a => a.State).HasColumnName("state").HasMaxLength(2);
            addr.Property(a => a.ZipCode).HasColumnName("zip_code").HasMaxLength(10);
        });

        builder.HasMany(p => p.Guardians)
            .WithOne()
            .HasForeignKey(g => g.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Campos educacionais/clínicos complementares (ajuste de cobertura —
        // NAPE A.2/A.3/A.4), todos opcionais e preenchidos aos poucos.
        builder.Property(p => p.Naturality).HasColumnName("naturality").HasMaxLength(100);
        builder.Property(p => p.CurrentSchool).HasColumnName("current_school").HasMaxLength(150);
        builder.Property(p => p.Grade).HasColumnName("grade").HasMaxLength(20);
        builder.Property(p => p.Shift).HasColumnName("shift").HasMaxLength(20);
        builder.Property(p => p.ClassGroup).HasColumnName("class_group").HasMaxLength(20);
        builder.Property(p => p.Zone).HasColumnName("zone").HasMaxLength(10);
        builder.Property(p => p.SchoolEnrollment).HasColumnName("school_enrollment").HasMaxLength(30);
        builder.Property(p => p.ReferredBySchool).HasColumnName("referred_by_school");

        // CSV ("TEA, TDAH") — RN32: evolução futura seria uma tabela N:N.
        builder.Property(p => p.DisabilityTypes).HasColumnName("disability_types").HasMaxLength(200);

        builder.Property(p => p.NeedsSpecialEducation).HasColumnName("needs_special_education");
        builder.Property(p => p.AttendsTutoring).HasColumnName("attends_tutoring");
        builder.Property(p => p.HasFailedGrade).HasColumnName("has_failed_grade");

        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex("name_normalized")
            .HasDatabaseName("idx_person_name_trgm")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.HasIndex(p => p.BirthDate)
            .HasDatabaseName("idx_person_birth_date");

        builder.HasIndex(p => p.Cns)
            .HasDatabaseName("idx_person_cns")
            .IsUnique()
            .HasFilter("cns IS NOT NULL");

        builder.HasIndex(p => p.Cpf)
            .HasDatabaseName("idx_person_cpf")
            .IsUnique()
            .HasFilter("cpf IS NOT NULL");
    }
}
