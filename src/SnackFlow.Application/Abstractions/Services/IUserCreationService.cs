using SnackFlow.Application.DTOs;

namespace SnackFlow.Application.Abstractions.Services;

public interface IUserCreationService
{
    /// <summary>
    /// Cria de forma assíncrona uma entidade de usuário completa com base nas informações fornecidas.
    /// </summary>
    /// <param name="dto">O objeto de transferência de dados contendo as informações necessárias para criar o usuário.</param>
    /// <param name="cancellationToken">Um token para observar solicitações de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task CreateCompleteUserAsync(CreateUserCompleteDTO dto, CancellationToken cancellationToken = default);
}