using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationRoleMap : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("aspnet_role");
        
        builder
            .HasKey(u => u.Id)
            .HasName("pk_aspnet_role_id");

        builder
            .Property(r => r.Id)
            .HasColumnType("uuid")
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        builder
            .Property(r => r.Name)
            .HasColumnType("text")
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(r => r.NormalizedName)
            .HasColumnType("text")
            .HasColumnName("normalized_name")
            .IsRequired();

        builder
            .HasIndex(r => r.NormalizedName)
            .HasDatabaseName("ix_aspnet_role_normalized_name")
            .IsUnique();
        
        builder
            .Property(r => r.ConcurrencyStamp)
            .HasColumnType("varchar(36)")
            .HasColumnName("concurrency_stamp")
            .IsConcurrencyToken();
    }
}