namespace SnackFlow.Domain.ValueObjects;

/// <summary>
/// Representa um recorde base abstrato para criação de objetos de valor imutáveis
/// que podem encapsular propriedades e comportamentos específicos do domínio.
/// </summary>
/// <remarks>
/// Esta classe deve ser herdada por outros records que definem
/// implementações específicas de objetos de valor dentro do domínio.
/// Ela serve como uma base compartilhada para objetos de valor, garantindo a imutabilidade
/// e oferecendo suporte a comportamentos específicos das propriedades do domínio.
/// </remarks>
public abstract record ValueObject;