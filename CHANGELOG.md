# Riber - Changelog

---

## v4.3.2 - 11/12/2025
**REFATORAÇÃO**: Mudança no result pattern e na classe de erro
- Remove a redundância de codigo na criação de um Error
- Remove sobrecargas na criação de Result com Errors
- Simplifica as respostas de Errors
- Remove método redundante do middleware de validação do token

---

## v4.3.1 - 09/12/2025
**REFATORAÇÃO**: Remove o método de busca por id da entidade nos IAiModelService
Adiciona relacionamento entre entidade `ProductEmbeddings` e `Company` para uma melhor busca

---

## v4.3.0 - 08/11/2025
- **NOVO**: Novas entidades par uso de chat e agendes de IA
- Adiciona entidade `Chat` para o usuário armazenar conversas com IA
- Adiciona entidade `ChatMessage` para armazenar mensagens dentro de um chat
- Adiciona a entidade `Assitant` junto com seeders de alguns assistente que terão na aplicacão
- **REFATORAÇÃO**: Mudança na organização de pasta das entidades e corrigindo os Namespace

---

## v4.2.1 - 07/11/2025
- **REFATORAÇÃO**: Unificar BaseEntity e BaseModel em Tracker para herança simplificada
- Remove `BaseEntityConfiguration` e `BaseModelConfiguration`, substituindo por `BaseTypeConfiguration`
- Introduz abstração `Tracker` consolidando propriedades e métodos comuns
- Atualiza modelos e mapeamentos para herdar de `Tracker`
- Simplifica lógica de auditoria em `AuditInterceptor` para manipular exclusivamente entidades `Tracker`
- Reorganiza mapeamentos seguindo a nova estrutura de herança

---

## v4.2.0 - 04/11/2025
- **NOVO**: Serviço para geração de Embeddings Genérico
- Adiciona um evento para gerar embeddings dos produtos
- Adiciona models ao banco onde referência as entidades para guardar os embeddings
- Adiciona um consumer para poder gerar os embeddings de forma assíncrona
- **TESTES**: Adiciona testes unitários para o novo consumer de geração de embeddings
- **REFATORAÇÃO**: Adapta o handler de produto para lançar evento de gerar embeddings do produto
- Refatora os teste do PermissionDataService para se tornarem de Integração em vez de Unitários
- Refatora os teste do AuditInterceptor para os casos de ModelBase ter SoftDelete

---

## v4.1.2 - 02/11/2025

- **REFATORAÇÃO**: Substituição de jobs de exclusão por arquitetura baseada em mensagens
- Remove `DeleteImageFromStorageDispatcher` e `DeleteImageFromStorageJob`
- Remove `DeleteImageFromStorageScheduler` obsoleto
- Adiciona `ProductImageCreationFailedEvent` para falhas na criação de imagens
- Implementa `ProductImageCreationFailedEventHandler` para publicação de mensagens
- Adiciona `ProductImageCreationFailedMessage` para comunicação assíncrona
- Implementa `ProductImageCreationFailedMessageConsumer` para exclusão assíncrona de imagens
- **TESTES**: Cobertura completa para novo consumidor de mensagens
- Adiciona testes unitários para `ProductImageCreationFailedMessageConsumer`
- Atualiza testes do `CreateProductCommandHandler` para novo fluxo
- **MELHORIA**: Validação aprimorada com `IsNullOrWhiteSpace` para chaves de imagem
- **MELHORIA**: Processamento de exclusão de imagens agora totalmente baseado em eventos e mensagens
- Sistema mais resiliente e escalável para gerenciamento de falhas

---

## v4.1.1 - 02/11/2025

- **NOVO**: Sistema de mensageria com MassTransit e RabbitMQ
- Adiciona `IMessagePublisher` para publicação de mensagens assíncronas
- Implementa `MassTransitMessagePublisher` para integração com MassTransit
- Adiciona `MassTransitPublishContextWrapper` para configuração de contexto de publicação
- Implementa `SendEmailMessageConsumer` para processamento assíncrono de e-mails
- Adiciona mensagem `SendEmailMessage` para comunicação entre serviços
- **REFATORAÇÃO**: Migração do sistema de envio de e-mails para arquitetura baseada em mensageria
- Remove implementação síncrona antiga de envio de e-mails
- Atualiza handlers para usar publicação de mensagens em vez de chamadas diretas
- Adiciona configuração do RabbitMQ no Docker Compose
- **MELHORIA**: Melhor separação de responsabilidades e desacoplamento entre camadas
- Sistema de e-mails agora opera de forma assíncrona e resiliente

---

## v4.1.0 - 26/10/2025

- **NOVO**: Sistema modular de serviços de autenticação com separação de responsabilidades
- Adiciona `IAuthenticationService` para operações de login e autenticação
- Adiciona `IRoleManagementService` para gerenciamento de roles
- Adiciona `IUserManagementService` para gerenciamento de usuários
- Adiciona `IUserQueryService` para consultas de usuários
- Implementa serviços de Identity: `AuthenticationService`, `RoleManagementService`, `UserManagementService`, `UserQueryService`, `UserMappingService` e `UserCreationService`
- **NOVO**: `EmptyResult` para operações sem retorno de valor
- **REFATORAÇÃO**: Reorganização das interfaces de serviços para pasta `Authentication`
- **REFATORAÇÃO**: Atualização de toda a aplicação (handlers, controllers, middlewares) para usar novos serviços
- **REFATORAÇÃO**: Melhorias no `PermissionDataService` e repositórios
- Remove serviços obsoletos: `IAuthService`, `AuthService`, `UserCreationService` (versão antiga)
- Remove `SpecificationExtension` não utilizada
- Atualiza todos os testes para refletir as mudanças arquiteturais

---

# v4.0.1 - 22/10/2025

- **CORREÇÃO**: Mudança no Job de limpeza de imagens na Bucket
- **MELHORIA**: Nova propriedade para mapear StatusCode no Result

---

## v4.0.0 - 19/10/2025

- **BREAKING CHANGE**: Reestruturação completa da resposta de erros da API
- Adicionada propriedade `type` para identificar o tipo do erro
- Alterada propriedade `messages` (array) para `message` (string única)
- Adicionada propriedade `details` (objeto) para erros de validação agrupados por campo
- Novo formato: `{ "type": "ERROR_TYPE", "message": "...", "details": { "Field": ["error1", "error2"] } }`
- **MELHORIA**: Erros de validação agora agrupam múltiplas mensagens por campo
- **MELHORIA**: Formato de erros mais alinhado com padrões modernos de API (inspirado em FastEndpoints)

---

## v3.0.1 - 18/10/2025

- CORREÇÃO: Troca do pacote de versionamento da API
- Ajuste no setup de testes da camada Api para testes de integração
- Ajuste no analyze.sh para ficar mais consistente a validação de ambiente

---

## v3.0.0 - 17/10/2025

- **REFATORAÇÃO**: Mudança na resposta da API, ajustando o Result Pattern
- Remoção da propriedade Details usada para mensagens de validação do FluentValidation
- Remoção da propriedade Message usada para mensagens resumidas
- Unificação dessas duas propriedades de erro para propriedade Messages
- **CORREÇÃO**: Ajustando as versões dos pacotes da aplicação

---

## v2.3.1 - 12/10/2025

- **NOVO**: Adiciona um novo job para limpeza de imagens não usadas na Bucket
- **CORREÇÃO**: Ajuste no validator de Criação de empresa com administrador
- Usar SHA da action em vez da versão
- **REFATORAÇÃO**: Padronização no cadastro dos Jobs
- Remoção das docs do projeto, centralizado tudo no README

---

## v2.3.0 - 09/10/25

- **NOVO**: Integração do SonarQube no ambiente de desenvolvimento
- Adiciona serviço SonarQube Community no Docker Compose com profile 'analysis'
- Configura banco de dados PostgreSQL dedicado para SonarQube
- Script de inicialização automática do banco SonarQube
- **REFATORAÇÃO**: Melhorias na entidade base e otimizações de código
- Melhora comparação de igualdade na BaseEntity com verificação de tipos
- Otimiza serialização JSON na extensão HttpContext
- Simplifica validações e melhora formatação geral do código
- **MELHORIAS**: Limpeza e padronização de código
- Remove código redundante e melhora legibilidade
- Otimiza imports e espaçamento consistente
- Pequenas otimizações de performance em behaviors e services

---

## v2.2.0 - 07/10/25

- **NOVO**: Sistema de diagnósticos e rastreamento distribuído
- Adiciona configuração centralizada para ActivitySource na camada Application
- Melhora significativamente o LoggingBehavior com suporte a distributed tracing
- **REFATORAÇÃO**: Otimização massiva de handlers e services
- Simplifica handlers de autenticação, empresas, usuários e categorias de produtos
- Otimiza services da camada Infrastructure (AuthService, PermissionDataService, AmazonSESService)
- Refatora Domain Specifications para melhor performance
- **MELHORIAS**: Padronização de código e configurações
- Melhora formatação XML em arquivos de configuração de pacotes
- Atualiza configurações de projeto seguindo padrões mais recentes
- Remove código redundante e melhora legibilidade geral
- **TESTES**: Cobertura abrangente para novas funcionalidades
- Adiciona testes completos para Activity tracing no LoggingBehavior
- Atualiza todos os testes para refletir código refatorado
- Melhora cenários de teste para tratamento de erros
- **DOCS**: Atualização da documentação de pacotes e dependências

---

## v2.1.1 - 05/10/25

- Adicionado suporte ao SonarQube para análise de qualidade de código
- Substituídos DTOs por Models no projeto para simplificar a lógica de dados

---

## v2.0.1 - 05/10/25

- **TEMPORÁRIO**: Deploy na AWS suspenso temporariamente

---

## v2.0.0 - 05/10/25

- **BREAKING CHANGE**: Reestruturação das mensagens de erro em módulos organizados
- Remove classe monolítica `ErrorMessage.cs` e substitui por estrutura modular
- Cria subdiretórios organizados: `Common/`, `Entities/` e `ValueObjects/`
- Atualiza mais de 50 arquivos nas camadas Application, Domain, Infrastructure e testes
- Melhora a manutenibilidade do código e organização das mensagens de erro
- **NOVO**: Sistema de gerenciamento de imagens para produtos
- Adiciona entidade `Image` com validações de tipo, tamanho e nome
- Implementa interface de armazenamento com suporte a AWS S3 e armazenamento local
- Cria exceções customizadas para tratamento de erros de imagens
- Adiciona migração do banco de dados para tabela de imagens
- Estabelece relacionamento entre Product e Image (1:N)
- **TEMPORÁRIO**: CreateProduct desativado para refatoração (será reimplementado na próxima PR)

---

## v1.0.3 - 29/09/25

- Retira os IsFailure do Result Pattern
- Ajustes os testes para remover o Result Pattern
- Correção no token de acesso do GHCR

---

## v1.0.2 - 29/09/25

- Ajusta o CD para usar tags
- Adiciona CHANGELOG.md

---

## v1.0.1 - 28/09/25

- Adiciona validação na criação de produto
- Adiciona testes que faltavam