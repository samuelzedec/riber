# 🔧 Design Patterns - SnackFlow

Este documento detalha todos os **design patterns** e **práticas arquiteturais** implementados no projeto SnackFlow, organizados conforme a estrutura atual das pastas.

---

## 📋 Índice
- [Patterns Arquiteturais](#patterns-arquiteturais)
- [Domain-Driven Design](#domain-driven-design)
- [CQRS + Mediator](#cqrs--mediator)
- [Repository + Unit of Work](#repository--unit-of-work)
- [Patterns de Validação](#patterns-de-validação)
- [Patterns de Infraestrutura](#patterns-de-infraestrutura)
- [Patterns de Tratamento de Erros](#patterns-de-tratamento-de-erros)
- [Organização e Estrutura](#organização-e-estrutura)

---

## 🏛️ Patterns Arquiteturais

### **Clean Architecture**
Organização em camadas com dependências sempre apontando para dentro.

```
┌─────────────────────────────────────┐
│            SnackFlow.Api            │  ← Apresentação
└─────────────────────────────────────┘
┌─────────────────────────────────────┐
│        SnackFlow.Application        │  ← Casos de Uso
└─────────────────────────────────────┘
┌─────────────────────────────────────┐
│          SnackFlow.Domain           │  ← Regras de Negócio
└─────────────────────────────────────┘
┌─────────────────────────────────────┐
│      SnackFlow.Infrastructure       │  ← Detalhes Externos
└─────────────────────────────────────┘
```

**Implementação:**
- **Domain**: Núcleo isolado sem dependências externas
- **Application**: Orquestra casos de uso sem conhecer infraestrutura
- **Infrastructure**: Implementa abstrações definidas no Domain
- **API**: Controllers que delegam para Application

---

## 🎯 Domain-Driven Design

### **Rich Domain Model**
Entidades com comportamento encapsulado, não apenas dados.

```
Domain/Entities/
└── Company.cs                    # Aggregate Root com regras de negócio
```

**Características:**
- Métodos de negócio na própria entidade
- Validações de invariantes no construtor
- Encapsulamento de estado interno

### **Value Objects**
Objetos imutáveis que representam conceitos do domínio.

```
Domain/ValueObjects/
├── Email/
│   ├── Email.cs                  # Imutável com validação
│   └── Exceptions/               # Exceções específicas
├── Phone/
├── CompanyName/
└── TaxId/
```

**Implementação:**
- Imutabilidade garantida
- Validação no construtor
- Exceções específicas por regra de negócio
- Comparação por valor, não referência

### **Domain Events**
Comunicação desacoplada entre agregados.

```
Domain/Events/
└── CompanyEmailValidationRequestedEvent.cs
```

**Benefícios:**
- Desacoplamento entre agregados
- Comunicação assíncrona
- Auditoria e rastreabilidade automática

### **Aggregate Pattern**
Garantia de consistência através de raízes de agregado.

```csharp
// Company é Aggregate Root
public class Company : Entity, IAggregateRoot
{
    // Controla acesso a entidades filhas
    // Garante invariantes do agregado
}
```

---

## 📨 CQRS + Mediator

### **Command Query Responsibility Segregation (CQRS)**
Separação entre operações de escrita e leitura.

```
Application/Features/Companies/
├── Commands/                     # Operações de escrita
│   └── CreateCompany/
│       ├── CreateCompanyCommand.cs
│       ├── CreateCompanyHandler.cs
│       ├── CreateCompanyResponse.cs
│       └── CreateCompanyValidator.cs
└── Queries/                      # Operações de leitura (futuro)
    └── GetCompanyById/
```

**Vantagens:**
- Modelos otimizados para diferentes operações
- Escalabilidade independente
- Clareza de responsabilidades

### **Mediator Pattern (MediatR)**
Desacoplamento através de mediador central.

```
Application/Abstractions/
├── Commands/
│   ├── ICommand.cs               # Contratos de comando
│   ├── ICommandHandler.cs        # Handlers de comando
│   └── ICommandResponse.cs       # Respostas padronizadas
└── Queries/
    ├── IQuery.cs                 # Contratos de consulta
    ├── IQueryHandler.cs          # Handlers de consulta
    └── IQueryResponse.cs         # Respostas de consulta
```

**Implementação:**
- Um handler por operação
- Interfaces tipadas e consistentes
- Injeção de dependência automática

### **Pipeline Behaviors**
Cross-cutting concerns através de pipeline.

```
Application/Behaviors/
├── ValidationBehavior.cs         # Validação automática com FluentValidation
└── LoggingBehavior.cs           # Logging de requests/responses
```

**Funcionalidades:**
- Validação automática antes da execução
- Logging estruturado de operações
- Extensível para cache, performance, etc.

---

## 🗄️ Repository + Unit of Work

### **Repository Pattern**
Abstração sobre acesso a dados com interfaces no domínio.

```
Domain/Repositories/
├── IRepository.cs                # Interface genérica
├── ICompanyRepository.cs         # Interface específica
└── IUnitOfWork.cs               # Controle transacional
```

```
Infrastructure/Persistence/Repositories/
├── BaseRepository.cs             # Implementação genérica
├── CompanyRepository.cs          # Implementação específica
└── UnitOfWork.cs                # Controle de transações
```

**Benefícios:**
- Testabilidade (mocks fáceis)
- Abstração sobre EF Core
- Queries específicas do domínio

### **Unit of Work Pattern**
Controle transacional centralizado.

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public ICompanyRepository Companies => 
        _companyRepository ??= new CompanyRepository(_context);
    
    public async Task<int> SaveChangesAsync() => 
        await _context.SaveChangesAsync();
}
```

**Características:**
- Uma transação por unidade de trabalho
- Lazy loading dos repositories
- Controle centralizado de persistência

---

## ✅ Patterns de Validação

### **Strategy Pattern**
Diferentes estratégias de validação encapsuladas.

```
Domain/Validators/DocumentValidator/
├── CnpjValidator.cs              # Estratégia para CNPJ
├── CpfValidator.cs               # Estratégia para CPF
└── Exceptions/                   # Exceções específicas
```

**Interface comum:**
```csharp
// Domain/Abstractions/
public interface IDocumentValidator
{
    bool IsValid(string document);
    void ValidateAndThrow(string document);
}
```

### **FluentValidation**
Validação fluente na camada de aplicação.

```
Application/Features/Companies/Commands/CreateCompany/
└── CreateCompanyValidator.cs     # Validação de entrada
```

**Pipeline automático:**
- ValidationBehavior intercepta commands
- Executa validação antes do handler
- Retorna erros estruturados

---

## 🏗️ Patterns de Infraestrutura

### **Factory Pattern**
Criação de objetos complexos com configuração adequada.

```
Infrastructure/Persistence/Factories/
└── AppDbContextFactory.cs        # Factory para DbContext design-time
```

**Uso:**
- Design-time para migrations
- Configuração específica por ambiente
- Injeção de dependências complexas

### **Interceptor Pattern**
Cross-cutting concerns no nível de persistência.

```
Infrastructure/Persistence/Interceptors/
├── AuditInterceptor.cs           # Campos CreatedAt/UpdatedAt automáticos
└── CaseInsensitiveInterceptor.cs # Configurações de case sensitivity
```

**Funcionalidades:**
- Auditoria automática de entidades
- Configurações globais de banco
- Interceptação de operações EF Core

### **Options Pattern**
Configurações tipadas e validadas.

```
Infrastructure/Settings/
├── AccessTokenSettings.cs        # Configurações JWT tipadas
└── RefreshTokenSettings.cs       # Configurações de refresh token
```

**Binding automático:**
```csharp
// Program.cs
builder.Services.Configure<AccessTokenSettings>(
    builder.Configuration.GetSection("AccessToken"));
```

---

## 🚨 Patterns de Tratamento de Erros

### **Result Pattern**
Tratamento de erros sem exceções para operações de negócio.

```
Application/Common/
├── Result.cs                     # Result<T> e Result base
└── ValidationError.cs            # Erros estruturados
```

**Implementação:**
```csharp
public class Result<T> : Result
{
    public T Value { get; }
    
    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static Result<T> Failure(Error error) => new(default, false, error);
}
```

**Vantagens:**
- Explicitação de possíveis falhas
- Composition de operações
- Evita exception-driven flow

### **Custom Exceptions Hierarchy**
Hierarquia organizada de exceções por contexto.

```
Domain/Exceptions/
├── DomainException.cs            # Base para domínio
└── UnsupportedCompanyTypeException.cs

Domain/ValueObjects/Email/Exceptions/
├── EmailFormatInvalidException.cs
└── EmailNullOrEmptyException.cs

Domain/Validators/DocumentValidator/Exceptions/
├── InvalidCnpjException.cs
└── InvalidCpfException.cs
```

**Organização:**
- Exceções gerais em `/Exceptions/`
- Exceções específicas junto ao código relacionado
- Hierarquia clara de herança

---

## 📁 Organização e Estrutura

### **Feature-Based Organization**
Organização por funcionalidade na camada de aplicação.

```
Application/Features/
├── Companies/                    # Tudo relacionado a Company
│   ├── Commands/
│   └── Queries/
├── Orders/                       # Futuro: Orders
└── Products/                     # Futuro: Products
```

**Benefícios:**
- Alta coesão por feature
- Fácil navegação e manutenção
- Times independentes por feature

### **Type-Based Organization**
Organização por tipo na camada de domínio.

```
Domain/
├── Entities/                     # Todos os agregados
├── ValueObjects/                 # Todos os value objects
├── Events/                       # Todos os domain events
├── Repositories/                 # Todas as interfaces
├── Validators/                   # Todos os validadores
└── Abstractions/                 # Todas as abstrações
```

**Vantagens:**
- Localização intuitiva por tipo
- Namespaces limpos e diretos
- Escalabilidade mantida

### **Separation of Concerns**
Cada camada com responsabilidade específica.

```
├── Api/                          # Apresentação e HTTP
├── Application/                  # Orquestração e casos de uso
├── Domain/                       # Regras de negócio puras
└── Infrastructure/               # Detalhes técnicos
```

**Princípios aplicados:**
- Single Responsibility Principle
- Dependency Inversion Principle
- Interface Segregation Principle

---

## 📊 Resumo dos Patterns

| Pattern | Localização | Propósito |
|---------|-------------|-----------|
| **Clean Architecture** | Estrutura geral | Separação de responsabilidades |
| **DDD** | Domain/ | Modelagem rica do domínio |
| **CQRS** | Application/Features/ | Separação comando/consulta |
| **Mediator** | Application/Abstractions/ | Desacoplamento de handlers |
| **Repository** | Domain/Repositories/ + Infrastructure/ | Abstração de persistência |
| **Unit of Work** | Infrastructure/Repositories/ | Controle transacional |
| **Strategy** | Domain/Validators/ | Algoritmos intercambiáveis |
| **Factory** | Infrastructure/Factories/ | Criação de objetos complexos |
| **Interceptor** | Infrastructure/Interceptors/ | Cross-cutting concerns |
| **Result** | Application/Common/ | Tratamento de erros explícito |
| **Options** | Infrastructure/Settings/ | Configurações tipadas |

---

## 🎯 Benefícios Alcançados

### **Manutenibilidade**
- Código organizado e previsível
- Responsabilidades bem definidas
- Facilidade para mudanças

### **Testabilidade**
- Abstrações mockáveis
- Lógica isolada por camada
- Comportamentos específicos testáveis

### **Escalabilidade**
- Estrutura preparada para crescimento
- Patterns que suportam complexidade
- Organização que facilita trabalho em equipe

### **Flexibilidade**
- Implementações intercambiáveis
- Configurações externalizadas
- Extensibilidade através de interfaces

---

*Este documento reflete a implementação atual dos patterns no projeto SnackFlow e será atualizado conforme a evolução da arquitetura.*