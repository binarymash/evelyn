namespace Evelyn.Host.Cli.Services
{
    using Evelyn.Agent;
    using global::FluentValidation;
    using Microsoft.Extensions.DependencyInjection;
    using Scrutor;

    public static class FluentValidation
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<AgentMarker>()
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
        }
    }
}
