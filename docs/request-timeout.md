# Políticas de Timeout

## Configuração

```csharp
builder.Services.AddRequestTimeouts(options => {
    options.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromMinutes(1) };
    options.AddPolicy("fast", TimeSpan.FromSeconds(15));
    options.AddPolicy("standard", TimeSpan.FromSeconds(30));
    options.AddPolicy("slow", TimeSpan.FromMinutes(1));
    options.AddPolicy("upload", TimeSpan.FromMinutes(5));
});
```

## Fast Policy (15 segundos)
**Quando usar:**
- Health checks
- Validações simples
- Consultas de cache
- Autenticação/autorização
- Operações que devem ser muito rápidas

```csharp
[HttpGet("health")]
[RequestTimeout("fast")]
public IActionResult HealthCheck() => Ok("Healthy");
```

## Standard Policy (30 segundos)
**Quando usar:**
- Operações CRUD básicas
- Consultas com filtros simples
- Processamento de formulários
- APIs externas padrão
- Maioria das operações da aplicação

```csharp
[HttpGet("users")]
[RequestTimeout("standard")]
public async Task<IActionResult> GetUsers() => Ok(await _service.GetUsersAsync());
```

## Slow Policy (1 minuto)
**Quando usar:**
- Relatórios complexos
- Consultas pesadas no banco
- Cálculos complexos
- Processamento de dados em lote
- Integração com sistemas legados

```csharp
[HttpPost("reports")]
[RequestTimeout("slow")]
public async Task<IActionResult> GenerateReport() => Ok(await _service.GenerateReportAsync());
```

## Upload Policy (5 minutos)
**Quando usar:**
- Upload de arquivos grandes
- Processamento de imagens/vídeos
- Import de planilhas (CSV/Excel)
- Operações de backup
- Qualquer processamento de arquivo

```csharp
[HttpPost("upload")]
[RequestTimeout("upload")]
public async Task<IActionResult> UploadFile(IFormFile file) => Ok(await _service.ProcessFileAsync(file));
```

## Default Policy (1 minuto)
**Quando usar:**
- Endpoints sem política específica
- Fallback para operações não categorizadas

```csharp
[HttpGet("data")]
// Sem [RequestTimeout] = usa a política padrão
public async Task<IActionResult> GetData() => Ok(await _service.GetDataAsync());
```