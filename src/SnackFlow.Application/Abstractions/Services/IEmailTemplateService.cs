using SnackFlow.Domain.Enums;

namespace SnackFlow.Application.Abstractions.Services;

/// <summary>
/// Define um serviço para gerenciar e gerar templates de e-mail dinamicamente com base nos dados e tipo de template especificados.
/// </summary>
/// <typeparam name="T">O tipo do modelo de dados a ser usado para renderizar o template.</typeparam>
public interface IEmailTemplateService<in T> where T : class
{
    /// <summary>
    /// Recupera e processa de forma assíncrona um template de email com base no público-alvo, tipo de template e modelo de dados especificados.
    /// Substitui os placeholders no template pelos valores correspondentes fornecidos nos dados.
    /// </summary>
    /// <param name="audience">O público-alvo do template de email, especificado como <see cref="EmailAudience"/>.</param>
    /// <param name="template">O tipo de template de email a ser recuperado, especificado como <see cref="EmailTemplate"/>.</param>
    /// <param name="data">O modelo de dados contendo as propriedades para substituir os placeholders no template.</param>
    /// <returns>Uma task que representa a operação assíncrona. O resultado da task contém o template de email processado como uma string.</returns>
    Task<string> GetTemplateAsync(EmailAudience audience, EmailTemplate template, T data);
}