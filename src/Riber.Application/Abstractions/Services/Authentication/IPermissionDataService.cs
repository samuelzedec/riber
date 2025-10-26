using Riber.Application.Common;
using Riber.Application.Models.Auth;

namespace Riber.Application.Abstractions.Services.Authentication;

/// <summary>
/// Serviço para gerenciamento e consulta de dados de permissões do sistema.
/// </summary>
public interface IPermissionDataService
{
    /// <summary>
    /// Verifica se uma permissão existe e está ativa no sistema.
    /// </summary>
    /// <param name="name">Nome da permissão a ser verificada.</param>
    /// <returns>Resultado da validação da permissão.</returns>
    Task<Result<EmptyResult>> ValidateAsync(string name);

    /// <summary>
    /// Alterna o status ativo/inativo de uma permissão.
    /// </summary>
    /// <param name="name">Nome da permissão a ser atualizada.</param>
    /// <returns>Resultado da operação de atualização.</returns>
    Task<Result<EmptyResult>> UpdatePermissionStatusAsync(string name);

    /// <summary>
    /// Retorna todas as permissões disponíveis com suas descrições.
    /// </summary>
    /// <returns>Coleção de permissões com detalhes e descrições.</returns>
    Task<Result<IReadOnlyCollection<PermissionModel>>> GetAllWithDescriptionsAsync();
}