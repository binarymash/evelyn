namespace Evelyn.Host.Cli.Services
{
    using System.Reflection;
    using Evelyn.Agent.Mediatr;
    using Evelyn.Agent.Mediatr.Behaviors;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;

    public static class MediatrPipeline
    {
        public static void AddMediatRPipeline(this IServiceCollection services)
        {
            services.AddMediatR(typeof(MediatorHandlers).GetTypeInfo().Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandling<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Logging<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Validation<,>));
        }
    }
}
