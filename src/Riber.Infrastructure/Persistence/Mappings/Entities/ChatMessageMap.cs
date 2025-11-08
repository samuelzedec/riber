using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.AI;
using ChatMessage = Riber.Domain.Entities.Chat.ChatMessage;

namespace Riber.Infrastructure.Persistence.Mappings.Entities;

public sealed class ChatMessageMap : BaseTypeConfiguration<ChatMessage>
{
    protected override string GetTableName()
        => "chat_message";

    protected override void Mapping(EntityTypeBuilder<ChatMessage> builder)
    {
        builder
            .Property(cm => cm.ChatId)
            .HasColumnName("chat_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .Property(cm => cm.Content)
            .HasColumnName("content")
            .HasColumnType("text")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(cm => cm.Role)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ChatRole>(v))
            .HasColumnName("role")
            .HasColumnType("text")
            .IsRequired();
    }
}