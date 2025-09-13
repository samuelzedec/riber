using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;

namespace Riber.Infrastructure.Persistence.Mappings;

public sealed class ProductCategoryMap : BaseEntityConfiguration<ProductCategory>
{
    protected override string GetTableName()
        => "product_category";

    protected override void ConfigureEntity(EntityTypeBuilder<ProductCategory> builder)
    {
        builder
            .Property(pc => pc.Name)
            .HasColumnName("name")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasIndex(pc => pc.Name, "ix_product_category_name");
        
        builder
            .Property(pc => pc.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(pc => pc.Code)
            .HasColumnName("code")
            .HasColumnType("text")
            .HasMaxLength(3)
            .IsRequired();
        
        builder
            .HasIndex(pc => new { pc.CompanyId, pc.Code }, "uq_product_category_company_code")
            .IsUnique();
        
        builder
            .Property(pc => pc.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired();
        
        builder
            .HasOne(pc => pc.Company)
            .WithMany()
            .HasForeignKey(pc => pc.CompanyId)
            .HasConstraintName("fk_product_category_company_id")
            .IsRequired();
        
        builder
            .Property(pc => pc.CompanyId)
            .HasColumnName("company_id")
            .HasColumnType("uuid")
            .IsRequired();
        
        builder
            .HasMany<Product>("_products")
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("fk_product_category_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .Metadata
            .FindNavigation("_products")!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Ignore(pc => pc.ProductsReadOnly);
    }
}