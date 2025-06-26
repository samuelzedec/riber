using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SnackFlow.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Um interceptor de comandos de banco de dados que modifica a execução de comandos SQL para realizar comparações
/// de strings sem diferenciar maiúsculas de minúsculas, substituindo ocorrências de "LIKE" por "ILIKE".
/// </summary>
/// <remarks>
/// Este interceptor é destinado ao uso com mecanismos de banco de dados que oferecem suporte à palavra-chave "ILIKE"
/// para comparações insensíveis a maiúsculas e minúsculas, como o PostgreSQL. Ele intercepta comandos que contêm
/// a palavra "LIKE" no texto SQL e a substitui por "ILIKE", garantindo esse comportamento.
/// </remarks>
/// <example>
/// Este interceptor pode ser registrado nas opções do DbContext para modificar globalmente o comportamento das consultas.
/// </example>
public sealed class CaseInsensitiveInterceptor : DbCommandInterceptor
{
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        ApplyCaseInsensitive(command);
        return base.ReaderExecutingAsync(
            command,
            eventData,
            result,
            cancellationToken
        );
    }
    
    private void ApplyCaseInsensitive(DbCommand command)
    {
        if (command.CommandText.Contains("LIKE"))
            command.CommandText = command.CommandText.Replace("LIKE", "ILIKE");    
    }
}
