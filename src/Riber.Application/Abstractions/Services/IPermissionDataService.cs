using Riber.Application.Models;

namespace Riber.Application.Abstractions.Services;

/// <summary>
/// Interface que define operações relacionadas ao gerenciamento de dados de permissões.
/// </summary>
public interface IPermissionDataService
{
    /// <summary>
    /// Valida a permissão com base no nome fornecido.
    /// </summary>
    /// <param name="name">O nome da permissão a ser validada.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa indica se a permissão é válida.</returns>
    Task<bool> ValidateAsync(string name);

    /// <summary>
    /// Atualiza o status da permissão com base no nome fornecido.
    /// </summary>
    /// <param name="name">O nome da permissão cujo status será atualizado.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    Task UpdatePermissionStatusAsync(string name);

    /// <summary>
    /// Obtém todas as permissões juntamente com suas descrições.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa é uma coleção de objetos PermissionDTO contendo os dados das permissões.</returns>
    Task<ICollection<PermissionModel>> GetAllWithDescriptionsAsync();
}