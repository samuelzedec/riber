using SnackFlow.Application.Abstractions.Events;
using SnackFlow.Application.Abstractions.Schedulers;
using SnackFlow.Application.DTOs.Email;
using SnackFlow.Application.Extensions;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.Events;

namespace SnackFlow.Application.Events.Handlers;

public sealed class CompanyWelcomeEmailRequestedEventHandler(
    IEmailScheduler emailScheduler)
    : IDomainEventHandler<CompanyWelcomeEmailRequestedEvent>
{
    public async ValueTask Handle(CompanyWelcomeEmailRequestedEvent notification, CancellationToken cancellationToken)
        => await emailScheduler.ScheduleEmailAsync(
            EmailAddress.NoReply,
            new WelcomeBaseEmailDTO(
                Audience: EmailAudience.Company.GetDescription(),
                Template: EmailTemplate.Welcome.GetDescription(),
                Name: notification.Name,
                Subject: "Seja bem-vindo ao SnackFlow!",
                To: "contact@samuelzedec.tech"
            ),
            cancellationToken
        );
}