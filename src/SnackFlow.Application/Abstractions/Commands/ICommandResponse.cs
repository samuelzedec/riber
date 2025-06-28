namespace SnackFlow.Application.Abstractions.Commands;

/// <summary>
/// Representa uma interface marcadora para respostas dentro de um framework de manipulação de comandos.
/// </summary>
/// <remarks>
/// Esta interface é usada para definir um contrato comum para respostas retornadas por operações de manipulação de comandos,
/// facilitando restrições genéricas em padrões CQRS e garantindo segurança de tipos.
/// </remarks>
public interface ICommandResponse;