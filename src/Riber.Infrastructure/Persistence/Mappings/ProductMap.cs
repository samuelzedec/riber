using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Infrastructure.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings;

public sealed class ProductMap : BaseEntityConfiguration<Product>
{
    protected override string GetTableName()
        => "product";

    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder
            .ConfigureUnitPrice()
            .ConfigureXmin();

        builder
            .HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .HasConstraintName("fk_product_company_id")
            .IsRequired();

        builder
            .Property(p => p.CompanyId)
            .HasColumnName("company_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(p => p.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(p => p.Name)
            .HasColumnName("name")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasIndex(p => p.Name, "ix_product_name");

        builder
            .Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired();

        builder
            .Property(p => p.ImageId)
            .HasColumnName("image_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder
            .HasOne(p => p.Image)
            .WithOne()
            .HasForeignKey<Product>(p => p.ImageId)
            .HasConstraintName("fk_product_image_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}