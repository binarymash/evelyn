// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Evelyn.Client;
    using Evelyn.Client.Repository;
    using Evelyn.Client.Synchronization;
    using Hosting;

    public static class EvelynClient
    {
#pragma warning disable SA1614 // Element parameter documentation must have text
#pragma warning disable SA1616 // Element return value documentation must have text
        /// <summary>
        /// Adds Evelyn feature toggling client services to the specified IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action">An Action&lt;EvelynClientOptions&gt; to configure the Evelyn client services</param>
        /// <returns></returns>
        public static IServiceCollection AddEvelynClient(this IServiceCollection services, Action<EvelynClientOptions> action)
#pragma warning restore SA1616 // Element return value documentation must have text
#pragma warning restore SA1614 // Element parameter documentation must have text
        {
        services.AddOptions();
            services.AddSingleton<IEvelynClient, Evelyn.Client.EvelynClient>();
            services.AddSingleton<IHostedService, EnvironmentStateSynchronizer>();
            services.AddSingleton<IEnvironmentStateRepository, InMemoryEnvironmentStateRepository>();

            action.Invoke(new EvelynClientOptions(services));

            return services;
        }
    }
}
