using MediatR;

namespace SnackFlow.Application.Abstractions.Events;

/// <summary>
/// Representa uma interface marcadora para eventos no nível de aplicação dentro do domínio.
/// Um evento de aplicação é usado para comunicar mudanças de estado, ações ou ocorrências
/// que são significativas para a camada de aplicação.
/// Esta interface estende a interface <see cref="INotification"/> fornecida pelo MediatR,
/// permitindo o padrão publish-subscribe na aplicação.
/// </summary>
public interface IApplicationEvent : INotification;