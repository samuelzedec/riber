using Riber.Application.Abstractions.Events;
using Riber.Application.Abstractions.Schedulers;
using Riber.Application.DTOs.Email;
using Riber.Application.Extensions;
using Riber.Domain.Enums;
using Riber.Domain.Events;

namespace Riber.Application.Events.Handlers;

internal sealed class CompanyWelcomeEmailRequestedEventHandler(
    IEmailScheduler emailScheduler)
    : IDomainEventHandler<CompanyWelcomeEmailRequestedEvent>
{
    public async ValueTask Handle(CompanyWelcomeEmailRequestedEvent notification, CancellationToken cancellationToken)
        => await emailScheduler.ScheduleEmailAsync(
            EmailAddress.NoReply,
            new WelcomeBaseEmailDTO(
                TemplatePath: $"{EmailAudience.Company.GetDescription()}-{EmailTemplate.Welcome.GetDescription()}",
                Name: notification.Name,
                Subject: "Seja bem-vindo ao Riber!",
                To: "contact@samuelzedec.tech"
            ),
            cancellationToken
        );
}