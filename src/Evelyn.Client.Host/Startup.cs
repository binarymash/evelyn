namespace Evelyn.Client.Host
{
    using System;
    using System.Threading;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder
                .AddSerilog(dispose: true)
                .SetMinimumLevel(LogLevel.Information));

            // set up Evelyn...
            services.AddEvelynClient(clientConfig =>
            {
                clientConfig.ProjectId = Guid.Parse("8f73d020-96c4-407e-8602-74fd4e2ed08b");
                clientConfig.Environment = "my-first-environment";
                clientConfig.SynchronizeEnvironmentStateUsing.Polling(pollingConfig =>
                {
                    pollingConfig.PollingPeriod = TimeSpan.FromSeconds(1);
                    pollingConfig.RetrieveEnvironmentStateUsing.RestProvider(restConfig =>
                    {
                        restConfig.BaseUrl = "http://evelyn-server-host";
                    });
                });
            });

            // set up our other application dependencies
            services.AddSingleton(typeof(ClassWithToggle));
            services.AddSingleton(typeof(Application));
        }

        public void OnStartup(ServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILogger<Startup>>();
            logger.LogInformation("Starting up...");

            // Let's start the background service that retrieves the toggle state
            var token = new CancellationToken(false);
            serviceProvider.GetService<IHostedService>().StartAsync(token);

            // ...and now lets kick off the rest of our application.
            var inputReader = serviceProvider.GetService<Application>();
            inputReader.Run();
        }
    }
}
