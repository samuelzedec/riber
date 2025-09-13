using Mediator;

namespace Riber.Domain.Abstractions;

/// <summary>
/// Representa um evento de domínio no contexto do padrão Domain-Driven Design (DDD).
/// Um evento de domínio é um evento que modela uma ocorrência significativa ou mudança
/// dentro do domínio. É usado para notificar assinantes ou partes do sistema sobre
/// essas ocorrências, permitindo que reajam adequadamente. Implementações desta interface
/// devem encapsular os detalhes de um evento específico dentro do domínio.
/// </summary>
public interface IDomainEvent : INotification;