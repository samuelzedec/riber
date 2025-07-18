using Mediator;
using SnackFlow.Domain.Abstractions;

namespace SnackFlow.Application.Abstractions.Events;

/// <summary>
/// Representa um manipulador de eventos de domínio no contexto do padrão Domain-Driven Design (DDD).
/// Um manipulador de eventos de domínio é responsável por encapsular a lógica que reage a
/// um determinado evento de domínio e processa as operações de negócio correspondentes.
/// Esta interface integra-se com a biblioteca MediatR para suportar notificações de eventos.
/// </summary>
/// <typeparam name="T">
/// O tipo do evento de domínio sendo manipulado. Deve implementar a interface <see cref="IDomainEvent"/>.
/// </typeparam>
public interface IDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;