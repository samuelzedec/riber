using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.ValueObjects.FullName;

namespace SnackFlow.Infrastructure.Persistence.Mappings;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder
            .HasKey(u => u.Id)
            .HasName("pk_user_id");

        builder
            .Property(u => u.Id)
            .HasColumnType("uuid")
            .HasColumnName("id")
            .IsRequired();

        builder
            .Property(u => u.CompanyId)
            .HasColumnType("uuid")
            .HasColumnName("company_id");

        builder
            .Property(u => u.Position)
            .HasColumnType("text")
            .HasColumnName("position")
            .HasConversion<string>()
            .IsRequired();

        builder
            .HasIndex(u => u.CompanyId, "ix_user_company_id");

        builder
            .HasOne(u => u.Company)
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
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

        builder.OwnsOne(u => u.TaxId, user =>
        {
            user
                .Property(c => c.Type)
                .HasColumnName("tax_id_type")
                .HasColumnType("text")
                .HasConversion<string>()
                .IsRequired();

            user
                .Property(c => c.Value)
                .HasColumnName("tax_id_value")
                .HasColumnType("text")
                .HasMaxLength(14)
                .IsRequired();

            user
                .HasIndex(c => c.Value, "uq_user_tax_id")
                .IsUnique();
        });

        builder.OwnsOne(u => u.FullName, user =>
        {
            user.Property(u => u.Value)
                .HasColumnType("text")
                .HasColumnName("full_name")
                .HasMaxLength(FullName.MaxLength)
                .IsRequired();
        });
    }
}