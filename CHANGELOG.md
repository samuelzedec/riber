# Riber - Changelog

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