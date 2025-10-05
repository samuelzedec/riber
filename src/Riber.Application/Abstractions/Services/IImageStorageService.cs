namespace Riber.Application.Abstractions.Services;

/// <summary>
/// Representa um serviço para gerenciamento de funcionalidades de armazenamento de imagens.
/// Fornece métodos para manipular o armazenamento, recuperação e gerenciamento de imagens.
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Faz upload de uma imagem para o serviço de armazenamento.
    /// </summary>
    /// <param name="stream">O stream da imagem a ser enviada.</param>
    /// <param name="fileName">O nome do arquivo a ser salvo no serviço de armazenamento.</param>
    /// <param name="contentType">O tipo MIME da imagem sendo enviada.</param>
    /// <returns>Uma task representando a operação assíncrona.</returns>
    Task UploadAsync(Stream stream, string fileName, string contentType);

    /// <summary>
    /// Recupera o stream de uma imagem armazenada no serviço de armazenamento local.
    /// </summary>
    /// <param name="fileName">O nome do arquivo da imagem a ser recuperada.</param>
    /// <returns>Uma task representando a operação assíncrona. O resultado da task contém o stream da imagem recuperada.</returns>
    Task<Stream> GetImageStreamAsync(string fileName);

    /// <summary>
    /// Remove uma imagem do serviço de armazenamento.
    /// </summary>
    /// <param name="fileName">O nome do arquivo da imagem a ser removida.</param>
    /// <returns>Uma task representando a operação assíncrona.</returns>
    Task DeleteAsync(string fileName);
}