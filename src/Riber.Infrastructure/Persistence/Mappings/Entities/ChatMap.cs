using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riber.Domain.Entities.Chat;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

public sealed class ChatMap : BaseTypeConfiguration<Chat>
{
    protected override string GetTableName()
        => "chat";

    protected override void Mapping(EntityTypeBuilder<Chat> builder)
    {
        builder
            .Property(c => c.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(c => c.AssistantId)
            .HasColumnName("assistant_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("fk_chat_user_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Assistant)
            .WithMany()
            .HasForeignKey(c => c.AssistantId)
            .HasConstraintName("fk_chat_assistant_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasIndex(c => new {c.UserId, c.AssistantId})
            .IsUnique()
            .HasDatabaseName("uq_chat_user_assistant_id");
        
        builder
            .HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .HasConstraintName("fk_chatmessage_chat_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}