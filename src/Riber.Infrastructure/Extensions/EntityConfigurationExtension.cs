using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.ValueObjects.CompanyName;
using Riber.Domain.ValueObjects.FullName;

namespace Riber.Infrastructure.Extensions;

public static class EntityConfigurationExtension
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
        builder.OwnsOne(x => x.FullName, fullName => fullName
            .Property(f => f.Value)
            .HasColumnType("text")
            .HasColumnName("full_name")
            .HasMaxLength(FullName.MaxLength)
            .IsRequired());
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

    public static EntityTypeBuilder<T> ConfigureRandomToken<T>(this EntityTypeBuilder<T> builder, string tableName,
        string columnName)
        where T : class, IHasRandomToken
    {
        builder.OwnsOne(x => x.Token, randomToken =>
        {
            randomToken
                .Property(i => i.Value)
                .HasColumnName(columnName)
                .HasColumnType("text")
                .HasMaxLength(100)
                .IsRequired();

            randomToken
                .HasIndex(i => i.Value, $"uq_{tableName}_{columnName}")
                .IsUnique();
        });
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureDiscount<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasDiscount
    {
        builder.OwnsOne(x => x.ItemDiscount, discount =>
        {
            discount
                .Property(d => d.Percentage)
                .HasColumnName("discount_percentage")
                .HasColumnType("numeric(4,2)")
                .HasDefaultValue(0m)
                .IsRequired();

            discount
                .Property(d => d.FixedAmount)
                .HasColumnName("discount_fixed_amount")
                .HasColumnType("numeric")
                .HasDefaultValue(0m)
                .IsRequired();

            discount
                .Property(d => d.Reason)
                .HasColumnName("discount_reason")
                .HasColumnType("text")
                .HasMaxLength(255)
                .IsRequired(false);
        }).Navigation(x => x.ItemDiscount).IsRequired(false);

        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureUnitPrice<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasUnitPrice
    {
        builder.OwnsOne(x => x.UnitPrice, money =>
        {
            money
                .Property(m => m.Value)
                .HasColumnName("unit_price")
                .HasColumnType("numeric")
                .IsRequired();

            money
                .Property(m => m.Currency)
                .HasColumnName("unit_price_currency")
                .HasColumnType("text")
                .HasMaxLength(3)
                .IsRequired();
        }).Navigation(x => x.UnitPrice).IsRequired();

        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureQuantity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasQuantity
    {
        builder.OwnsOne(x => x.Quantity, quantity => quantity
            .Property(q => q.Value)
            .HasColumnName("quantity")
            .HasColumnType("integer")
            .IsRequired()).Navigation(x => x.Quantity).IsRequired();
        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureXmin<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasXmin
    {
        builder
            .Property(x => x.XminCode)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureContentType<T>(this EntityTypeBuilder<T> builder)
        where T : class, IHasContentType
    {
        builder.OwnsOne(x => x.ContentType, contentType => contentType
            .Property(c => c.Value)
            .HasColumnName("content_type")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired()
        ).Navigation(x => x.ContentType).IsRequired();

        return builder;
    }
}