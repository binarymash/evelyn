﻿namespace Evelyn.Client.Host
{
    using System;
    using System.Threading;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
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
                        restConfig.BaseUrl = "http://localhost:2316";
                    });
                });
            });

            // set up our other application dependencies
            services.AddSingleton(typeof(OutputWriter));
            services.AddSingleton(typeof(InputReader));
        }

        public void OnStartup(ServiceProvider serviceProvider)
        {
            // Let's start the background service that retrieves the toggle state
            var token = new CancellationToken(false);
            serviceProvider.GetService<IHostedService>().StartAsync(token);

            // ...and now lets kick off the rest of our application.
            var inputReader = serviceProvider.GetService<InputReader>();
            inputReader.Run();
        }
    }
}