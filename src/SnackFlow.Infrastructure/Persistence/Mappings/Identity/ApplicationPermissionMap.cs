using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Mappings.Identity;

public class ApplicationPermissionMap
    : IEntityTypeConfiguration<ApplicationPermission>
{
    public void Configure(EntityTypeBuilder<ApplicationPermission> builder)
    {
        builder.ToTable("application_permissions");

        builder
            .HasKey(ap => ap.Id)
            .HasName("pk_application_permissions_id");

        builder
            .Property(ap => ap.Id)
            .HasColumnType("integer")
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder
            .Property(ap => ap.Name)
            .HasColumnType("varchar")
            .HasColumnName("name")
            .HasMaxLength(256)
            .IsRequired();
        
        builder
            .HasIndex(ap => ap.Name, "uq_application_permission_name")
            .IsUnique();

        builder
            .Property(ap => ap.Category)
            .HasColumnType("varchar")
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(ap => ap.IsActive)
            .HasColumnType("boolean")
            .HasColumnName("is_active")
            .HasDefaultValue(true);
    }
}