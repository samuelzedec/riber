using ChefControl.Domain.CompanyContext.Entities;
using ChefControl.Domain.CompanyContext.ValueObjects.CompanyName;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChefControl.Infrastructure.Persistence.Mappings;

public class CompanyMap : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        #region Primitive Properties

        builder.ToTable("company");
        
        builder
            .HasKey(c => c.Id)
            .HasName("pk_company_id");

        builder
            .Property(c => c.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();
        
        builder
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder
            .Property(c => c.ModifiedAt)
            .HasColumnName("modified_at")
            .HasColumnType("timestamptz");
        
        builder
            .Property(c => c.DeletedAt)
            .HasColumnName("Deleted_at")
            .HasColumnType("timestamptz");

        #endregion
        
        #region Value Objects
        
        builder.OwnsOne(c => c.CompanyName, company =>
        {
            company
                .Property(c => c.Name)
                .HasColumnName("name")
                .HasColumnType("text")
                .HasMaxLength(CompanyName.NameMaxLength)
                .HasAnnotation("MinLength", CompanyName.MinLength)
                .IsRequired();

            company
                .HasIndex(c => c.Name, "uq_company_name");
            
            company
                .Property(c => c.TradingName)
                .HasColumnName("trading_name")
                .HasColumnType("text")
                .HasMaxLength(CompanyName.TradingNameMaxLength)
                .HasAnnotation("MinLength", CompanyName.MinLength)
                .IsRequired();
            
        });

        builder.OwnsOne(c => c.TaxId, company =>
        {
            company
                .Property(c => c.Type)
                .HasColumnName("type")
                .HasColumnType("text")
                .HasConversion<string>()
                .IsRequired();
            
            company
                .Property(c => c.Value)
                .HasColumnName("value")
                .HasColumnType("text")
                .HasMaxLength(14)
                .IsRequired();
            
            company
                .HasIndex(c => c.Value, "uq_company_tax_id")
                .IsUnique();
        });

        builder.OwnsOne(c => c.Email, company =>
        {
            company
                .Property(c => c.Value)
                .HasColumnName("email")
                .HasColumnType("text")
                .HasMaxLength(255)
                .IsRequired();
            
            company
                .HasIndex(c => c.Value, "uq_company_email")
                .IsUnique();
        });
        
        builder.OwnsOne(c => c.Phone, company =>
        {
            company
                .Property(c => c.Value)
                .HasColumnName("phone")
                .HasColumnType("text")
                .HasMaxLength(15)
                .IsRequired();
            
            company
                .HasIndex(c => c.Value, "uq_company_phone")
                .IsUnique();
        });
        
        #endregion
        
        #region Query Filter Global
        
        builder.HasQueryFilter(c => !c.DeletedAt.HasValue);
        
        #endregion
    }
}