namespace Riber.Domain.Constants.Messages.Common;

public static class StorageErrors
{
    public const string AccessDenied = "Serviço de armazenamento temporariamente indisponível. Tente novamente mais tarde.";
    public const string BucketNotFound = "Erro de configuração de armazenamento. Entre em contato com o suporte.";
    public const string UploadFailed = "Falha ao enviar a imagem. Tente novamente mais tarde.";
    public const string ImageNotFound = "Imagem não encontrada.";
    public const string RetrieveAccessDenied = "Serviço de armazenamento temporariamente indisponível. Tente novamente mais tarde.";
    public const string RetrieveFailed = "Falha ao recuperar a imagem. Tente novamente mais tarde.";
}