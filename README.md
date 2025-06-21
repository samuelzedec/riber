# 🍔 Chef Control
Olá! Me chamo **Samuel Ribeiro** e este é o backend que estou desenvolvendo para a aplicação de gestão de uma
**lanchonete local em Manaus-AM 🍔🍟**.

O sistema está sendo criado com o objetivo de oferecer à lanchonete um controle mais eficiente das operações e das finanças, permitindo gerenciar receitas, despesas e fluxo de caixa de forma simples e organizada.

## 📦 Pacotes e Dependências

### 🔗 Abstrações
Pacotes que contém apenas contratos e interfaces, sem implementações concretas.

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `Microsoft.Extensions.Logging.Abstractions` | 9.0.5 | Abstrações para sistema de logging |
| `MediatR.Contracts` | 2.0.1 | Contratos do padrão Mediator |

### 🎯 Application Layer
Pacotes específicos da camada de aplicação, responsável por orquestrar as regras de negócio.

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `FluentValidation` | 12.0.0 | Biblioteca para validação fluente de objetos |
| `FluentValidation.DependencyInjectionExtensions` | 12.0.0 | Extensões para injeção de dependência do FluentValidation |
| `MediatR` | 12.5.0 | Implementação do padrão Mediator para CQRS |

### 🏗️ Infrastructure Layer
Pacotes da camada de infraestrutura, responsável por persistência, logging e segurança.

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `Serilog.AspNetCore` | 9.0.0 | Framework de logging estruturado |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 9.0.5 | ASP.NET Core Identity com Entity Framework |
| `Microsoft.EntityFrameworkCore` | 9.0.4 | ORM para acesso a dados |
| `Microsoft.EntityFrameworkCore.Relational` | 9.0.4 | Funcionalidades relacionais do EF Core |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.4 | Provider PostgreSQL para Entity Framework |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.4 | Ferramentas de design-time do EF Core |

### 🌐 API Layer
Pacotes específicos da camada de apresentação (API Web).

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.5 | Autenticação via JWT Bearer tokens |
| `Microsoft.AspNetCore.OpenApi` | 9.0.5 | Suporte para documentação OpenAPI/Swagger |

### 🧪 Testes
Ferramentas e bibliotecas para testes automatizados.

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `FluentAssertions` | 8.3.0 | Biblioteca para assertions mais legíveis |
| `Moq` | 4.20.72 | Framework para criação de mocks |
| `Bogus` | 35.6.3 | Gerador de dados fake para testes |
| `Microsoft.NET.Test.Sdk` | 17.14.0 | SDK base para testes .NET |
| `xunit` | 2.9.3 | Framework de testes unitários |
| `xunit.runner.visualstudio` | 3.1.0 | Runner do xUnit para Visual Studio |
| `Microsoft.AspNetCore.Mvc.Testing` | 9.0.5 | Ferramentas para testes de integração |
| `Moq.EntityFrameworkCore` | 9.0.0.5 | Extensões do Moq para Entity Framework |
| `MockQueryable.Moq` | 7.0.3 | Mocks para IQueryable com Moq |

## 🏛️ Arquitetura

O projeto segue os princípios de **Clean Architecture**, organizando as dependências em camadas bem definidas:

- **Domain**: Entidades e regras de negócio puros
- **Application**: Casos de uso e orquestração
- **Infrastructure**: Implementações de persistência e serviços externos
- **API**: Controladores e configurações web

## 🛠️ Tecnologias Principais

- **.NET 9.0** - Framework principal
- **PostgreSQL** - Banco de dados
- **Entity Framework Core** - ORM
- **ASP.NET Core Identity** - Autenticação e autorização
- **MediatR** - Padrão Mediator/CQRS
- **FluentValidation** - Validações
- **JWT** - Tokens de autenticação
- **Serilog** - Logging estruturado
- **xUnit** - Testes unitários