# Riber - Backend
Olá! Me chamo **Samuel Ribeiro** e este é o backend que estou desenvolvendo para a aplicação de gestão de uma
**lanchonete local em Manaus-AM**.

O sistema está sendo criado com o objetivo de oferecer à lanchonete um controle mais eficiente das operações e das finanças, permitindo gerenciar receitas, despesas e fluxo de caixa de forma simples e organizada.

## Arquitetura

O projeto segue os princípios de **Clean Architecture**, organizando as dependências em camadas bem definidas:

- **Domain**: Entidades e regras de negócio puros
- **Application**: Casos de uso e orquestração
- **Infrastructure**: Implementações de persistência e serviços externos
- **API**: Controladores e configurações web

## Tecnologias Principais

- **.NET 9.0** - Framework principal
- **PostgreSQL** - Banco de dados
- **Entity Framework Core** - ORM
- **ASP.NET Core Identity** - Autenticação e autorização
- **Mediator** - Padrão Mediator/CQRS
- **FluentValidation** - Validações
- **JWT** - Tokens de autenticação
- **Serilog** - Logging estruturado
- **xUnit** - Testes unitários

## Documentação

Para informações detalhadas sobre desenvolvimento e padrões do projeto, consulte:

- **[Padrões de Codificação](docs/coding-standards.md)** - Convenções de código, CQRS, testes e organização
- **[Padrões Arquiteturais](docs/design-patterns.md)** - Repository, UnitOfWork, DDD e outros padrões utilizados
- **[Pacotes e Dependências](docs/packages.md)** - Lista completa de bibliotecas e suas finalidades
- **[Configuração de Timeout](docs/request-timeout.md)** - Configurações de timeout para requisições

## Usuários Padrão no banco de dados

### Administrador: 
- UserName = admin123 
- Password = Admin@123

### Diretor: 
- UserName = director123 
- Password = Director@123

## Executando com Docker Compose

### 3. Executando os serviços

```bash
# Subir todos os serviços
docker-compose up -d

# Ver logs em tempo real
docker-compose logs

# Parar os serviços
docker-compose down --rmi all --volumes
```

### 4. Comandos úteis

```bash
# Rebuildar e subir
docker-compose up -d --build

# Subir apenas o banco
docker-compose up postgres

# Ver status dos containers
docker-compose ps
```
