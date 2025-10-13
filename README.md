# Riber - Backend

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=samuelzedec_riber&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=samuelzedec_riber)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=samuelzedec_riber&metric=coverage)](https://sonarcloud.io/summary/new_code?id=samuelzedec_riber)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=samuelzedec_riber&metric=bugs)](https://sonarcloud.io/summary/new_code?id=samuelzedec_riber)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=samuelzedec_riber&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=samuelzedec_riber)

Olá! Me chamo **Samuel Ribeiro** e este é o backend que estou desenvolvendo para a aplicação de gestão de uma
**lanchonete local em Manaus-AM**.

---

## A História do Projeto


Este projeto nasceu como uma oportunidade de **aplicar e consolidar conhecimentos** adquiridos ao longo da minha jornada como desenvolvedor backend, além de **explorar novas tecnologias e padrões** de forma prática e autodirecionada.

Desenvolvo este sistema no meu **tempo livre**, com o objetivo de criar uma solução real que possa ser utilizada por uma **pequena lanchonete da minha família** quando finalizado.

O foco está em implementar **boas práticas**, **arquitetura limpa** e **padrões modernos** de desenvolvimento, criando um projeto de portfólio que demonstra capacidade técnica e compromisso com qualidade de código.

---

## Arquitetura

O projeto segue os princípios de **Clean Architecture**, organizando as dependências em camadas bem definidas:

- **Domain**: Entidades e regras de negócio puros
- **Application**: Casos de uso e orquestração
- **Infrastructure**: Implementações de persistência e serviços externos
- **API**: Controladores e configurações web

---

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

---

## Usuários Padrão no banco de dados

### Administrador: 
- UserName = admin123 
- Password = Admin@123

### Diretor: 
- UserName = director123 
- Password = Director@123

---

## Executando com Docker Compose

### 3. Executando os serviços

```bash
# Subir todos os serviços
docker compose up -d

# Subir todos os serviçõs e o SonarQube
docker compose up -d --profile analysis

# Ver logs em tempo real
docker compose logs

# Parar os serviços
docker compose down --rmi all --volumes
```

### 4. Comandos úteis

```bash
# Rebuildar e subir
docker compose up -d --build

# Subir apenas o banco
docker compose up postgres

# Ver status dos containers
docker compose ps
```
