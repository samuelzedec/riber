using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.ValueObjects.CompanyName;

namespace SnackFlow.Infrastructure.Persistence.Mappings;

public class CompanyMap : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
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
            .Property(c => c.UpdatedAt)
            .HasColumnName("modified_at")
            .HasColumnType("timestamptz");
        
        builder
            .Property(c => c.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamptz");
        
        builder.OwnsOne(c => c.Name, company =>
        {
            company
                .Property(c => c.Corporate)
                .HasColumnName("corporate_name")
                .HasColumnType("text")
                .HasMaxLength(CompanyName.CorporateMaxLength)
                .HasAnnotation("MinLength", CompanyName.MinLength)
                .IsRequired();

            company
                .HasIndex(c => c.Corporate, "uq_company_corporate_name");
            
            company
                .Property(c => c.Fantasy)
                .HasColumnName("fantasy_name")
                .HasColumnType("text")
                .HasMaxLength(CompanyName.FantasyMaxLength)
                .HasAnnotation("MinLength", CompanyName.MinLength)
                .IsRequired();
            
        });

        builder.OwnsOne(c => c.TaxId, company =>
        {
            company
                .Property(c => c.Type)
                .HasColumnName("tax_id_type")
                .HasColumnType("text")
                .HasConversion<string>()
                .IsRequired();
            
            company
                .Property(c => c.Value)
                .HasColumnName("tax_id_value")
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
        
        builder.HasQueryFilter(c => !c.DeletedAt.HasValue);
    }
}