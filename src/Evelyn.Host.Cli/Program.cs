namespace Evelyn.Host.Cli
{
    using System;
    using System.Threading.Tasks;
    using Evelyn.Host.Cli.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(() =>
            {
                BootstrapServices()
                    .GetService<Application>()
                    .Run()
                    .Wait();
            });
        }

        private static IServiceProvider BootstrapServices()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostApplication();
            services.AddConfiguration();
            services.AddConsoleLogging();
            services.AddMediatRPipeline();
            services.AddFluentValidation();
        }
    }
}