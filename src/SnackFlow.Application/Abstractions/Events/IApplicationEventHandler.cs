using Mediator;

namespace SnackFlow.Application.Abstractions.Events;

/// <summary>
/// Representa um manipulador de eventos no nível de aplicação que implementa a interface <see cref="IApplicationEvent"/>.
/// Um manipulador de eventos de aplicação é responsável por processar eventos específicos da camada
/// de aplicação e executar a lógica de negócio correspondente.
/// Esta interface integra-se com a biblioteca MediatR para suportar o padrão de notificação de eventos.
/// </summary>
/// <typeparam name="T">
/// O tipo do evento de aplicação sendo manipulado. Deve implementar a interface <see cref="IApplicationEvent"/>.
/// </typeparam>
public interface IApplicationEventHandler<in T> : INotificationHandler<T>
    where T : IApplicationEvent;
