using Riber.Domain.Entities.Abstractions;
using Entity = Riber.Domain.Entities.User;

namespace Riber.Domain.Entities.Chat;

public sealed class Chat : BaseEntity
{
    #region Properties

    public Guid UserId { get; init; }
    public Guid AssistantId { get; init; }

    #endregion

    #region Navigation Properties

    public Entity.User User { get; private set; } = null!;
    public Assistant Assistant { get; private set; } = null!;
    public ICollection<ChatMessage> Messages { get; private set; } = null!;

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private Chat() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private Chat(Guid userId, Guid assistantId) : base(Guid.CreateVersion7())
    {
        UserId = userId;
        AssistantId = assistantId;
    }

    #endregion

    #region Factories

    public static Chat Create(Guid userId, Guid assistantId)
        => new(userId, assistantId);

    #endregion

    #region Methods

    public void AddMessage(ChatMessage message)
    {
        Messages.Add(message);
        UpdateEntity();
    }

    #endregion
}