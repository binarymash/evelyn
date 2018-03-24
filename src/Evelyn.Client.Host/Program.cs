namespace Evelyn.Client.Host
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            // setup the service dependencies...
            IServiceCollection services = new ServiceCollection();

            services.AddEvelynClient(eve =>
            {
                eve.SynchronizationOptions(config =>
                {
                    config.Environment = "development";
                    config.ProjectId = Guid.Parse("{222649E0-1E2D-4A1A-B986-3400CEC08B49}");
                    config.PollingPeriod = TimeSpan.FromSeconds(5);
                });

                eve.RetrieveEnvironmentStateUsing.RestProvider(config =>
                {
                    config.BaseUrl = "http://localhost:2316";
                });
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
