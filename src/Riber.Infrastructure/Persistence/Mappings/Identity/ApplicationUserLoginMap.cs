using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationUserLoginMap : IEntityTypeConfiguration<ApplicationUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
    {
        builder.ToTable("aspnet_user_login");
        
        builder
            .HasKey(ul => new { ul.UserId, ul.LoginProvider, ul.ProviderKey })
            .HasName("pk_aspnet_user_login_user_id_login_provider_provider_key");
        
        builder
            .Property(ul => ul.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .IsRequired();
        
        builder
            .Property(ul => ul.LoginProvider)
            .HasColumnType("text")
            .HasColumnName("login_provider")
            .IsRequired();
        
        builder
            .Property(ul => ul.ProviderKey)
            .HasColumnType("text")
            .HasColumnName("provider_key")
            .IsRequired();

        builder
            .Property(ul => ul.ProviderDisplayName)
            .HasColumnType("text")
            .HasColumnName("provider_display_name")
            .IsRequired(false);
    }
}