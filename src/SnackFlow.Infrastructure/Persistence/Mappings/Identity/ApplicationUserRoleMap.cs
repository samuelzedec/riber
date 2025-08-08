using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnackFlow.Infrastructure.Persistence.Identity;

namespace SnackFlow.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationUserRoleMap : IEntityTypeConfiguration<ApplicationUserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ToTable("aspnet_user_roles");

        builder
            .HasKey(ur => new { ur.UserId, ur.RoleId })
            .HasName("pk_aspnet_user_role_user_id_role_id");
            
        builder
            .Property(ur => ur.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .IsRequired();
        
        builder
            .Property(ur => ur.RoleId)
            .HasColumnType("uuid")
            .HasColumnName("role_id")
            .IsRequired();

        builder
            .HasIndex(ur => ur.UserId)
            .HasDatabaseName("ix_aspnet_user_role_user_id");

        builder
            .HasIndex(ur => ur.RoleId)
            .HasDatabaseName("ix_aspnet_user_role_role_id");
    }
}