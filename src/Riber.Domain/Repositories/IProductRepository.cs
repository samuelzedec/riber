using System.Linq.Expressions;
using Riber.Domain.Entities;
using Riber.Domain.Entities.Catalog;
using Riber.Domain.Specifications.Core;

namespace Riber.Domain.Repositories;

/// <summary>
/// Representa uma interface de repositório específica para gerenciamento de produtos.
/// Estende a interface genérica <see cref="IRepository&lt;T&gt;"/>, tendo <see cref="Product"/> como entidade raiz de agregado.
/// Fornece métodos para operações de persistência e acesso a dados relacionados aos produtos.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Cria assincronamente uma nova categoria de produto no armazenamento de dados.
    /// </summary>
    /// <param name="productCategory">
    /// A entidade de categoria de produto a ser criada. Deve conter todas as propriedades obrigatórias com valores válidos.
    /// </param>
    /// <param name="cancellationToken">
    /// Um token de cancelamento para observar enquanto aguarda a conclusão da tarefa.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona de criação da categoria do produto.
    /// </returns>
    Task CreateCategoryAsync(ProductCategory productCategory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma categoria de produto existente no armazenamento de dados.
    /// </summary>
    /// <param name="productCategory">
    /// A entidade de categoria de produto a ser atualizada. Deve conter valores de propriedades válidos e atualizados.
    /// </param>
    void UpdateCategory(ProductCategory productCategory);

    /// <summary>
    /// Recupera assincronamente uma categoria de produto que atende aos critérios de uma especificação fornecida.
    /// </summary>
    /// <param name="specification">
    /// A especificação que define os critérios para a seleção da categoria de produto.
    /// </param>
    /// <param name="cancellationToken">
    /// Um token de cancelamento para observar enquanto aguarda a conclusão da tarefa.
    /// </param>
    /// <param name="includes">
    /// Uma coleção opcional de expressões para especificar quais propriedades relacionadas devem ser incluídas ao buscar a categoria.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona, retornando uma instância de <see cref="ProductCategory"/> correspondente à especificação fornecida,
    /// ou nulo se nenhuma categoria atender aos critérios.
    /// </returns>
    Task<ProductCategory?> GetCategoryAsync(Specification<ProductCategory> specification,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategory, object>>[] includes);

    /// <summary>
    /// Recupera assincronamente uma lista de categorias de produtos que atendem à especificação fornecida.
    /// </summary>
    /// <param name="specification">
    /// A especificação usada para filtrar as categorias de produtos. Define os critérios que as categorias devem atender.
    /// </param>
    /// <param name="cancellationToken">
    /// Um token de cancelamento para observar enquanto aguarda a conclusão da tarefa.
    /// </param>
    /// <param name="includes">
    /// Um array opcional de expressões para incluir propriedades relacionadas nas categorias retornadas.
    /// </param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona de recuperação das categorias de produtos.
    /// O resultado da tarefa contém uma coleção das categorias que atendem à especificação fornecida.
    /// </returns>
    Task<IEnumerable<ProductCategory>> GetCategoriesAsync(
        Specification<ProductCategory> specification,
        CancellationToken cancellationToken = default,
        params Expression<Func<ProductCategory, object>>[] includes);

    /// <summary>
    /// Cria e armazena a imagem especificada no serviço de armazenamento.
    /// </summary>
    /// <param name="image">A imagem que deve ser criada e salva.</param>
    /// <param name="cancellationToken">Um <see cref="CancellationToken"/> para observar durante a execução da operação.</param>
    /// <returns>Uma <see cref="Task"/> que é concluída quando a imagem é criada e armazenada.</returns>
    Task CreateImageAsync(Image image, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as imagens que não estão sendo utilizadas no sistema.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    /// <returns>Uma coleção de imagens não utilizadas.</returns>
    Task<IReadOnlyList<Image>> GetUnusedImagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma imagem específica do serviço de armazenamento.
    /// </summary>
    /// <param name="image">A imagem que deve ser removida.</param>
    void DeleteImage(Image image);
}