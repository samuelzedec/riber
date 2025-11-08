using Riber.Domain.Entities.Abstractions;
using Riber.Domain.Enums;

namespace Riber.Domain.Entities.Chat;

public sealed class Assistant : BaseEntity
{
    #region Properities

    public string Name { get; init; }
    public string SystemPrompt { get; init; }
    public AssistantType Type { get; init; }

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private Assistant() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private Assistant(Guid id, string name, string systemPrompt, AssistantType type)
        : base(id)
    {
        Name = name;
        SystemPrompt = systemPrompt;
        Type = type;
    }

    #endregion

    #region Factories

    public static Assistant Create(Guid id, string name, string systemPrompt, AssistantType type)
        => new(id, name, systemPrompt, type);

    #endregion

    #region Operators

    public static implicit operator string(Assistant assistant)
        => assistant.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => SystemPrompt;

    #endregion
}