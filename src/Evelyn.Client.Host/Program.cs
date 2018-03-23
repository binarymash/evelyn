namespace Evelyn.Client.Host
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Rest;

    public class Program
    {
        public static void Main(string[] args)
        {
            // setup the service dependencies...
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IEvelynClient, EvelynClient>();
            services.AddSingleton<IHostedService, EnvironmentStateSynchronizer>();
            services.AddSingleton<IEnvironmentStateProvider, EnvironmentStateRestProvider>();
            services.AddSingleton<IEnvironmentStateRepository, InMemoryEnvironmentStateRepository>();

            var serviceProvider = services.BuildServiceProvider();

            // our actual program...
            var token = new CancellationToken(false);
            serviceProvider.GetService<IHostedService>().StartAsync(token);

            var evelyn = serviceProvider.GetService<IEvelynClient>();

            bool? lastValue = null;
            while (true)
            {
                var toggleState = evelyn.GetToggleState("my-first-toggle").Result;
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
