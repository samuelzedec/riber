# Riber - Changelog

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