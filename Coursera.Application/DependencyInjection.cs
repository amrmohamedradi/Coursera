using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Coursera.Application.Common.Behaviors;

namespace Coursera.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                // Automatically run FluentValidation validators for every
                // Command / Query before its handler executes.
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}

