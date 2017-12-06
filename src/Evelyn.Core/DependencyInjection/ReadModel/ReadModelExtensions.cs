namespace Microsoft.Extensions.DependencyInjection
{
    using System;

    public static class ReadModelExtensions
    {
        ////private static Action<ReadModelRegistration> defaultConfiguration = (config) =>
        ////{
        ////    config.WithReadStrategy.ReadFromMemoryCache();
        ////};

        ////public static IServiceCollection AddEvelynReadModel(this IServiceCollection services)
        ////{
        ////    return AddEvelynReadModel(services, defaultConfiguration);
        ////}

        public static IServiceCollection AddEvelynReadModel(this IServiceCollection services, Action<ReadModelRegistration> action)
        {
            action.Invoke(new ReadModelRegistration(services));

            return services;
        }
    }
}
