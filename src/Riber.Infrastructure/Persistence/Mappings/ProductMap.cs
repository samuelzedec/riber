using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities;
using Riber.Infrastructure.Persistence.Extensions;

namespace Riber.Infrastructure.Persistence.Mappings;

public sealed class ProductMap : BaseEntityConfiguration<Product>
{
    protected override string GetTableName()
        => "product";

    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.ConfigureUnitPrice();
        
        builder
            .HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .HasConstraintName("fk_product_company_id")
            .IsRequired();
        
        builder
            .Property(x => x.CompanyId)
            .HasColumnName("company_id")
            .HasColumnType("uuid")
            .IsRequired();
        
        builder
            .Property(x => x.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uuid")
            .IsRequired();
        
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasIndex(x => x.Name, "ix_product_name");
        
        builder
            .Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired();

        builder
            .Property(x => x.ImageUrl)
            .HasColumnName("image_url")
            .HasColumnType("text")
            .IsRequired(false);
    }
}