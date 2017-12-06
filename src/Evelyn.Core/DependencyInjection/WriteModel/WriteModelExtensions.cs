namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using CQRSlite.Caching;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Events;
    using CQRSlite.Routing;
    using Evelyn.Core.WriteModel;

    public static class WriteModelExtensions
    {
        public static IServiceCollection AddEvelynWriteModel(this IServiceCollection services, Action<WriteModelRegistration> action)
        {
            services
                .AddCoreWriteInfrastructure()
                .AddCommandHandlers();

            action.Invoke(new WriteModelRegistration(services));

            return services;
        }

        private static IServiceCollection AddCoreWriteInfrastructure(this IServiceCollection services)
        {
            return services
                .AddSingleton<Router>(new Router())
                .AddSingleton<ICommandSender>(serviceProvider => serviceProvider.GetService<Router>())
                .AddSingleton<IEventPublisher>(serviceProvider => serviceProvider.GetService<Router>())
                .AddSingleton<IHandlerRegistrar>(serviceProvider => serviceProvider.GetService<Router>())

                // session
                .AddScoped<ISession, Session>()
                .AddScoped<IRepository>(CacheRepositoryFactory)
                .AddSingleton<ICache, MemoryCache>();
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
