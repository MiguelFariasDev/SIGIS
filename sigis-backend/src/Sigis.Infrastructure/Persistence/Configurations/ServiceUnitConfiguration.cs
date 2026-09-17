using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Persistence.Configurations;

/// <summary>
/// Mapeamento relacional de <see cref="ServiceUnit"/> — unidade de serviço
/// da rede municipal (NASF, NAPE, CREAES, Casa Mais Azul, CRASF).
/// </summary>
public sealed class ServiceUnitConfiguration : IEntityTypeConfiguration<ServiceUnit>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ServiceUnit> builder)
    {
        builder.ToTable("service_unit");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");

        builder.Property(u => u.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Acronym).HasColumnName("acronym").HasMaxLength(20).IsRequired();

        builder.Property(u => u.Secretariat)
            .HasColumnName("secretariat")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(u => u.Acronym).IsUnique().HasDatabaseName("idx_service_unit_acronym");
    }
}
