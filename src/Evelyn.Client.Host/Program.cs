namespace Evelyn.Client.Host
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Provider;
    using Repository;
    using Rest;
    using Synchronization;

    public class Program
    {
        public static void Main(string[] args)
        {
            // setup the service dependencies...
            IServiceCollection services = new ServiceCollection();

            // register client
            services.AddSingleton<IEvelynClient, EvelynClient>();

            //register synchronization
            services.AddSingleton<IHostedService, EnvironmentStateSynchronizer>();
            services.Configure<EnvironmentStateSynchronizerOptions>(options =>
            {
                options.Environment = "development";
                options.ProjectId = Guid.Parse("{222649E0-1E2D-4A1A-B986-3400CEC08B49}");
                options.SynchronizationPeriod = TimeSpan.FromSeconds(5);
            });

            //register repository
            services.AddSingleton<IEnvironmentStateRepository, InMemoryEnvironmentStateRepository>();

            //register provider
            services.AddSingleton<IEnvironmentStateProvider, EnvironmentStateRestProvider>();
            services.Configure<EnvironmentStateRestProviderOptions>(options =>
            {
                options.BaseUrl = "http://localhost:2316";
            });

            var serviceProvider = services.BuildServiceProvider();

            // our actual program...
            var token = new CancellationToken(false);
            serviceProvider.GetService<IHostedService>().StartAsync(token);

            var evelyn = serviceProvider.GetService<IEvelynClient>();

            bool? lastValue = null;
            while (true)
            {
                var toggleState = evelyn.GetToggleState("my-first-toggle");
                if (!lastValue.HasValue || lastValue.Value != toggleState)
                {
                    Console.WriteLine($"Toggle state is now {toggleState}");
                    lastValue = toggleState;
                }

                Task.Delay(TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();
            }
        }
    }
}
