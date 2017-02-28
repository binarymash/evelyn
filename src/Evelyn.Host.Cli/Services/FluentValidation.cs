namespace Evelyn.Host.Cli.Services
{
    using Evelyn.Agent.Features.Locations.Get;
    using global::FluentValidation;
    using Microsoft.Extensions.DependencyInjection;

    public static class FluentValidation
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddTransient<IValidator<Query>>((a) => { return new Validator(); });
        }
    }
}
