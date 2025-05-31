using ChefControl.Application.SharedContext.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ChefControl.Application;

/// <summary>
/// Fornece métodos e configurações para configurar o container de injeção de dependência da camada de Application.
/// Esta classe tem como objetivo simplificar o processo de registro de serviços de casos de uso,
/// validações e regras de negócio necessárias para o funcionamento da aplicação.
/// </summary>
public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configure.AddBehavior(typeof(ValidationBehavior<,>));
            configure.AddBehavior(typeof(LoggingBehavior<,>));
        });
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}