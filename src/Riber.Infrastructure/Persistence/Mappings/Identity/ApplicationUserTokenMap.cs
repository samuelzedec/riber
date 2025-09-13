using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Infrastructure.Persistence.Identity;

namespace Riber.Infrastructure.Persistence.Mappings.Identity;

public sealed class ApplicationUserTokenMap : IEntityTypeConfiguration<ApplicationUserToken>
{
    public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
    {
        builder.ToTable("aspnet_user_token");

        builder
            .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name })
            .HasName("pk_aspnet_user_token_user_id_login_provider_name");

        builder
            .Property(ut => ut.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .IsRequired();

        builder
            .Property(ut => ut.LoginProvider)
            .HasColumnType("text")
            .HasColumnName("login_provider")
            .IsRequired();

        builder
            .Property(ut => ut.Name)
            .HasColumnType("text")
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(ut => ut.Value)
            .HasColumnType("text")
            .HasColumnName("value")
            .IsRequired();
    }
}