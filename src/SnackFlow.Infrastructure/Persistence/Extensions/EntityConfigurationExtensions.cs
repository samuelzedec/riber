using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Domain.Abstractions.ValueObjects;
using SnackFlow.Domain.ValueObjects.CompanyName;
using SnackFlow.Domain.ValueObjects.FullName;

namespace SnackFlow.Infrastructure.Persistence.Extensions;

public static class EntityConfigurationExtensions
{
    public static EntityTypeBuilder<T> ConfigureTaxId<T>(this EntityTypeBuilder<T> builder, string indexName)
        where T : class, IHasTaxId
    {
        builder.OwnsOne(x => x.TaxId, taxId =>
        {
            taxId.Property(t => t.Type)
                .HasColumnName("tax_id_type")
                .HasColumnType("text")
                .HasConversion<string>()
                .IsRequired();

            taxId.Property(t => t.Value)
                .HasColumnName("tax_id_value")
                .HasColumnType("text")
                .HasMaxLength(14)
                .IsRequired();

            taxId.HasIndex(t => t.Value, indexName)
                .IsUnique();
        });
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureEmail<T>(this EntityTypeBuilder<T> builder, string indexName)
        where T : class, IHasEmail
    {
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasColumnType("text")
                .HasMaxLength(255)
                .IsRequired();

            email.HasIndex(e => e.Value, indexName)
                .IsUnique();
        });
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigurePhone<T>(this EntityTypeBuilder<T> builder, string indexName)
        where T : class, IHasPhone
    {
        builder.OwnsOne(x => x.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("phone")
                .HasColumnType("text")
                .HasMaxLength(15)
                .IsRequired();

            phone.HasIndex(p => p.Value, indexName)
                .IsUnique();
        });
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureFullName<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasFullName
    {
        builder.OwnsOne(x => x.FullName, fullName =>
        {
            fullName.Property(f => f.Value)
                .HasColumnType("text")
                .HasColumnName("full_name")
                .HasMaxLength(FullName.MaxLength)
                .IsRequired();
        });
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureCompanyName<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasCompanyName
    {
        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(n => n.Corporate)
                .HasColumnName("corporate_name")
                .HasColumnType("text")
                .HasMaxLength(CompanyName.CorporateMaxLength)
                .IsRequired();

            name.HasIndex(n => n.Corporate, "uq_company_corporate_name");

            name.Property(n => n.Fantasy)
                .HasColumnName("fantasy_name")
                .HasColumnType("text")
                .HasMaxLength(CompanyName.FantasyMaxLength)
                .IsRequired();
        });
        return builder;
    }
}