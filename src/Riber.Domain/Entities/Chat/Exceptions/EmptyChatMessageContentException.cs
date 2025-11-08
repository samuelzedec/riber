using Riber.Domain.Exceptions;

namespace Riber.Domain.Entities.Chat.Exceptions;

public sealed class EmptyChatMessageContentException(string message) 
    : DomainException(message);