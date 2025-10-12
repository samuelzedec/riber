using Riber.Application.Abstractions.Dispatchers;
using Riber.Application.Abstractions.Events;
using Riber.Application.Extensions;
using Riber.Application.Models.Email;
using Riber.Domain.Enums;
using Riber.Domain.Events;

namespace Riber.Application.Events.Handlers;

internal sealed class CompanyWelcomeEmailRequestedEventHandler(
    IEmailDispatcher emailDispatcher)
    : IDomainEventHandler<CompanyWelcomeEmailRequestedEvent>
{
    public async ValueTask Handle(CompanyWelcomeEmailRequestedEvent notification, CancellationToken cancellationToken)
        => await emailDispatcher.SendAsync(
            EmailAddress.NoReply,
            new WelcomeBaseEmailModel(
                TemplatePath: $"{EmailAudience.Company.GetDescription()}-{EmailTemplate.Welcome.GetDescription()}",
                Name: notification.Name,
                Subject: "Seja bem-vindo ao Riber!",
                To: "contact@samuelzedec.tech"
            ),
            cancellationToken
        );
}