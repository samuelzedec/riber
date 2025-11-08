using Microsoft.Extensions.AI;
using Riber.Domain.Constants.Messages.Entities;
using Riber.Domain.Entities.Abstractions;
using Riber.Domain.Entities.Chat.Exceptions;

namespace Riber.Domain.Entities.Chat;

public sealed class ChatMessage : BaseEntity
{
    #region Properties

    public Guid ChatId { get; init; }
    public string Content { get; init; }
    public ChatRole Role { get; init; }

    #endregion

    #region Navigation Properties

    public Chat Chat { get; private set; } = null!;

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private ChatMessage() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private ChatMessage(Guid chatId, string content, ChatRole role)
        : base(Guid.CreateVersion7())
    {
        ChatId = chatId;
        Content = content;
        Role = role;
    }

    #endregion

    #region Factories

    public static ChatMessage Create(Guid chatId, string content, ChatRole role)
    {
        return string.IsNullOrWhiteSpace(content)
            ? throw new EmptyChatMessageContentException(ChatErrors.ContentMessageEmpty)
            : new ChatMessage(chatId, content, role);
    }

    #endregion

    #region Overrides

    public static implicit operator string(ChatMessage chatMessage)
        => chatMessage.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Content;

    #endregion
}