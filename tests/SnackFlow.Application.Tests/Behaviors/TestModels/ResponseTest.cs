using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Tests.Behaviors.TestModels;

/// <summary>
/// Representa um modelo de resposta usado nos comportamentos de teste da aplicação.
/// </summary>
/// <remarks>
/// Este registro é usado para encapsular os dados de resposta em cenários de teste.
/// Implementa a interface marcadora <see cref="ICommandResponse"/> para 
/// indicar seu papel como uma resposta de comando.
/// </remarks>
/// <param name="Name">O nome da entidade de resposta.</param>
public record ResponseTest(string Name) : ICommandResponse;