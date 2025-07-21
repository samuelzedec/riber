# SnackFlow - Padrões de Codificação

Este documento define as convenções e padrões de código para o projeto SnackFlow.

## CQRS Patterns

### Commands
**Estrutura:** `public sealed record {Ação}{Entidade}Command(...) : ICommand<{Response}>;`

```csharp
// ✅ Correto
public sealed record CreateCompanyCommand(string Name, string TaxId) : ICommand<CreateCompanyResponse>;

// ❌ Incorreto
public class CreateCompanyCommand { } // deve ser sealed record
public record CreateCompanyCommand { } // deve ser sealed
```

### Command Handlers
**Estrutura:** `internal sealed class {Command}Handler(...) : ICommandHandler<{Command}, {Response}>`

```csharp
// ✅ Correto
internal sealed class CreateCompanyHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateCompanyCommand, CreateCompanyResponse>

// ❌ Incorreto
public sealed class CreateCompanyHandler { } // deve ser internal
public class CreateCompanyHandler { } // deve ser internal sealed
```

### Command Responses
**Estrutura:** `public sealed record {Command}Response(...) : ICommandResponse;`

```csharp
// ✅ Correto
public sealed record CreateCompanyResponse(Guid CompanyId, string Name) : ICommandResponse;

// ❌ Incorreto
public class CreateCompanyResponse { } // deve ser sealed record
```

### Queries
**Estrutura:** `public sealed record {Descrição}Query(...) : IQuery<{Response}>;`

```csharp
// ✅ Correto
public sealed record GetCompanyByIdQuery(Guid Id) : IQuery<GetCompanyResponse>;
public sealed record GetAllCompaniesQuery() : IQuery<List<GetCompanyResponse>>;
```

### Query Handlers
**Estrutura:** `internal sealed class {Query}Handler(...) : IQueryHandler<{Query}, {Response}>`

```csharp
// ✅ Correto
internal sealed class GetCompanyByIdHandler(ICompanyRepository repository)
    : IQueryHandler<GetCompanyByIdQuery, GetCompanyResponse>

// ❌ Incorreto
public sealed class GetCompanyByIdHandler { } // deve ser internal
```

### Query Responses
**Estrutura:** `public sealed record {Descrição}Response(...) : IQueryResponse;`

### Validators
**Estrutura:** `internal sealed class {Command/Query}Validator : AbstractValidator<{Command/Query}>`

```csharp
// ✅ Correto
internal sealed class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>

// ❌ Incorreto
public sealed class CreateCompanyValidator { } // deve ser internal
public class CreateCompanyValidator { } // deve ser internal sealed
```

## Testes

### Nomenclatura de Testes
- **Testes Unitários:** `{ClasseTeste}UnitTest.cs`
- **Testes de Integração:** `{ClasseTeste}IntegrationTest.cs`
- **Testes End-to-End:** `{ClasseTeste}E2ETest.cs`

```csharp
// ✅ Exemplos corretos
CompanyValidatorUnitTest.cs
CompanyRepositoryIntegrationTest.cs
CreateCompanyE2ETest.cs

// ❌ Incorreto
CompanyTest.cs // não especifica o tipo
CompanyUnitTests.cs // plural incorreto
```

### Estrutura de Métodos de Teste
**Padrão:** `{Ação}_{Condição}_{ResultadoEsperado}`

```csharp
// ✅ Correto
[Fact(DisplayName = "Creating company when data is valid should return success")]
public void CreatingCompany_WhenDataIsValid_ShouldReturnSuccess()

[Fact(DisplayName = "Validating company when CNPJ is invalid should throw exception")]
public void ValidatingCompany_WhenCnpjIsInvalid_ShouldThrowException()

// ❌ Incorreto
public void TestCreateCompany() // não segue o padrão
public void CreateCompany_ShouldWork() // muito vago
```

### Organização de Testes
```
Tests/
├── SnackFlow.Domain.Tests/           (UnitTest - lógica pura)
├── SnackFlow.Application.Tests/      (UnitTest - casos de uso)
└── SnackFlow.Infrastructure.Tests/   (IntegrationTest - banco, APIs)
```

### Categorização por Regions
```csharp
public class CompanyValidatorUnitTest
{
    #region Fields and Setup
    // Campos e configuração inicial
    #endregion

    #region Valid Cases Tests
    // Testes de casos válidos
    #endregion

    #region Invalid Cases Tests
    // Testes de casos inválidos
    #endregion

    #region Edge Cases Tests
    // Testes de casos extremos
    #endregion
}
```

### Acesso a Classes Internas para Testes
Para testar handlers e validators internos, configure `InternalsVisibleTo`:

```xml
<!-- No .csproj da Application -->
<ItemGroup>
    <InternalsVisibleTo Include="SnackFlow.Application.Tests" />
    <InternalsVisibleTo Include="SnackFlow.IntegrationTests" />
</ItemGroup>
```

```csharp
// Exemplo de teste acessando classe interna
public class CreateCompanyHandlerUnitTest
{
    [Fact]
    public async Task Handle_WhenValidCommand_ShouldCreateCompany()
    {
        // ✅ Funciona com InternalsVisibleTo configurado
        var handler = new CreateCompanyHandler(mockUnitOfWork);
        
        // teste...
    }
}
```

## Entidades

### Estrutura Base
- Herdar de `BaseEntity`
- Usar factory methods estáticos para criação
- Construtor privado ou protegido

```csharp
// ✅ Correto
public class Company : BaseEntity
{
    private Company() { } // construtor privado
    
    public static Company Create(string name, string taxId)
    {
        // validações e criação
    }
}

// ❌ Incorreto
public class Company // deve herdar de BaseEntity
{
    public Company() { } // construtor público
}
```

## Value Objects

### Estrutura
- Herdar de `BaseValueObject`
- Usar factory methods estáticos para criação
- Construtor privado
- **Usar `sealed`** (exceto se for estendido)

```csharp
// ✅ Correto
public sealed partial record Email : BaseValueObject
{
    public string Value { get; }
    
    private Email(string value) => Value = value;
    
    public static Email Create(string value)
    {
        // validações e criação
    }
}

// ❌ Incorreto
public class Email // deve ser record e herdar de BaseValueObject
{
    public Email() { } // construtor público
}
```

### Organização com Regions
Usar `#region` para organizar o código dos Value Objects:

```csharp
public sealed partial record Email : BaseValueObject
{
    #region Properties
    // Propriedades
    #endregion

    #region Constants
    // Constantes (regex patterns, etc.)
    #endregion
    
    #region Constructors
    // Construtores privados
    #endregion
    
    #region Factories
    // Métodos estáticos de criação
    #endregion

    #region Source Generator
    // Regex gerados, etc.
    #endregion

    #region Operators
    // Operadores implícitos
    #endregion

    #region Overrides
    // ToString, GetHashCode, etc.
    #endregion
}
```

## Convenções Gerais

### Modificadores de Acesso
- **Commands/Queries/Responses:** `public sealed record`
- **Handlers:** `internal sealed class`
- **Validators:** `internal sealed class`
- **Entidades:** `public class`
- **Value Objects:** `public sealed record` (ou `public record` se for estendido)

### Justificativa para Handlers/Validators Internos
- **Encapsulamento**: Implementações são detalhes internos da camada Application
- **API Limpa**: Apenas contratos (Commands/Queries/Responses) são expostos
- **Flexibilidade**: Mudanças internas não afetam consumidores externos
- **Testabilidade**: Mantida através de `InternalsVisibleTo`

### Injeção de Dependências
- Use **primary constructors** quando possível
- Prefira interfaces sobre classes concretas

```csharp
// ✅ Correto
internal sealed class Handler(IUnitOfWork unitOfWork)

// ❌ Evitar (quando primary constructor é possível)
internal sealed class Handler
{
    private readonly IUnitOfWork _unitOfWork;
    public Handler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
}
```

### Nomenclatura
- **Classes:** PascalCase
- **Métodos:** PascalCase
- **Propriedades:** PascalCase
- **Campos privados:** camelCase com underscore (`_fieldName`)
- **Parâmetros:** camelCase
- **Variáveis locais:** camelCase

### Estrutura de Pastas
```
Application/
├── Commands/
│   └── CreateCompany/
│       ├── CreateCompanyCommand.cs      # public sealed record
│       ├── CreateCompanyHandler.cs      # internal sealed class
│       ├── CreateCompanyResponse.cs     # public sealed record
│       └── CreateCompanyValidator.cs    # internal sealed class
└── Queries/
    └── GetCompanyById/
        ├── GetCompanyByIdQuery.cs       # public sealed record
        ├── GetCompanyByIdHandler.cs     # internal sealed class
        └── GetCompanyByIdResponse.cs    # public sealed record
```

## Exceções

### Nomenclatura
- Terminar com `Exception`
- Herdar de exceção base apropriada

```csharp
// ✅ Correto
public sealed class CompanyNotFoundException : NotFoundException

// ❌ Incorreto
public sealed class CompanyNotFound // deve terminar com Exception
```

## Validações

### FluentValidation
- Um validator por Command/Query (`internal sealed class`)
- Mensagens de erro em português
- Usar `WithMessage()` para customizar mensagens

```csharp
// ✅ Correto
internal sealed class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome da empresa é obrigatório")
            .MaximumLength(100)
            .WithMessage("Nome deve ter no máximo 100 caracteres");
    }
}
```

## Resumo de Modificadores

| Tipo | Modificador | Justificativa |
|------|-------------|---------------|
| **Commands/Queries** | `public sealed record` | Contratos da API da camada |
| **Responses** | `public sealed record` | Contratos da API da camada |
| **Handlers** | `internal sealed class` | Detalhes de implementação |
| **Validators** | `internal sealed class` | Detalhes de implementação |
| **Entidades** | `public class` | Parte do modelo de domínio |
| **Value Objects** | `public sealed record` | Parte do modelo de domínio |
| **Interfaces** | `public interface` | Contratos públicos |