namespace Riber.Application.Messages;

/// <summary>
/// Representa uma mensagem que indica a falha na criação de uma imagem de produto.
/// Esta mensagem contém informações sobre a chave da imagem associada ao processo de criação que falhou.
/// </summary>
/// <remarks>
/// O <see cref="ProductImageCreationFailedMessage"/> é normalmente usado em contextos de mensageria 
/// onde uma falha na criação de uma imagem de produto precisa ser comunicada ou tratada.
/// </remarks>
public sealed record ProductImageCreationFailedMessage(string ImageKey);