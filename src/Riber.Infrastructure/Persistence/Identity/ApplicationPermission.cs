namespace Riber.Infrastructure.Persistence.Identity;

/// <summary>
/// Representa as permissões associadas a uma aplicação.
/// </summary>
public sealed class ApplicationPermission
{
    /// <summary>
    /// Obtém ou define o identificador único para a permissão do aplicativo.
    /// Esta propriedade é usada para distinguir entre diferentes permissões dentro do sistema.
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Obtém ou define o nome da permissão do aplicativo.
    /// Esta propriedade representa o nome único que identifica uma permissão específica.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a descrição da permissão do aplicativo.
    /// Esta propriedade fornece detalhes sobre o que a permissão específica implica ou permite.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define a categoria da permissão do aplicativo.
    /// Esta propriedade especifica a classificação ou agrupamento ao qual a permissão pertence.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Obtém ou define um valor indicando se a permissão do aplicativo está ativa.
    /// Esta propriedade determina se a permissão específica está atualmente habilitada ou acessível.
    /// </summary>
    public bool IsActive { get; set; }
}