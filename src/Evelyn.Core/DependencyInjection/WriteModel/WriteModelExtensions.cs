// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using CQRSlite.Caching;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.WriteModel;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class WriteModelExtensions
    {
        public static IServiceCollection AddEvelynWriteModel(this IServiceCollection services, Action<WriteModelRegistration> action)
        {
            services
                .AddCoreWriteInfrastructure()
                .AddCommandHandlers();
                ////.AddRouting();

            action.Invoke(new WriteModelRegistration(services));

            return services;
        }

        private static IServiceCollection AddCoreWriteInfrastructure(this IServiceCollection services)
        {
            services.TryAddSingleton<Router>(new Router());
            services.TryAddSingleton<ICommandSender>(serviceProvider => serviceProvider.GetService<Router>());
            services.TryAddSingleton<IEventPublisher>(serviceProvider => serviceProvider.GetService<Router>());
            services.TryAddSingleton<IHandlerRegistrar>(serviceProvider => serviceProvider.GetService<Router>());

            // session
            services.TryAddScoped<ISession, Session>();
            services.TryAddScoped<IRepository>(CacheRepositoryFactory);
            services.TryAddSingleton<ICache, MemoryCache>();

            return services;
        }

        private static IRepository CacheRepositoryFactory(IServiceProvider serviceProvider)
        {
            return new CacheRepository(
                new Repository(serviceProvider.GetService<IEventStore>()),
                serviceProvider.GetService<IEventStore>(),
                serviceProvider.GetService<ICache>());
        }

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            return services.Scan(scan => scan
                .FromAssemblyOf<WriteModelAssemblyMarker>()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }
    }
}
