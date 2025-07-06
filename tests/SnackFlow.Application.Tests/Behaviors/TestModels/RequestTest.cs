using SnackFlow.Application.Abstractions.Commands;

namespace SnackFlow.Application.Tests.Behaviors.TestModels;

/// <summary>
/// Representa um registro de teste para uma requisição que implementa a interface <see cref="ICommand{TCommandResponse}"/>.
/// </summary>
/// <remarks>
/// Este registro é utilizado para cenários de teste na camada de aplicação.
/// Inclui propriedades básicas como <c>Name</c> e <c>Age</c>,
/// e serve como um modelo de requisição que produz um <see cref="ResponseTest"/> após o processamento.
/// </remarks>
/// <param name="Name">O nome associado à requisição.</param>
/// <param name="Age">A idade associada à requisição.</param>
public record RequestTest(
    string Name,
    int Age
) : ICommand<ResponseTest>;