namespace Evelyn.Host.Cli
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using BinaryMash.Responses;
    using Evelyn.Agent.Features.Locations;
    using Evelyn.Agent.Features.Locations.Get.Model;
    using Evelyn.Agent.Features.Locations.Get.Validation;
    using Evelyn.Agent.Mediatr;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var handler = serviceProvider.GetService<IAsyncRequestHandler<Query, Response<Locations>>>();

            var application = serviceProvider.GetService<Application>();

            Task.Run(() => application.Run()).Wait();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            AddLogging(services);
            AddConfiguration(services);
            AddValidation(services);

            services.AddMediatR(typeof(MediatorHandlers).GetTypeInfo().Assembly);
            services.AddTransient<Application>();
        }

        private static void AddLogging(IServiceCollection services)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            services.AddSingleton(loggerFactory);
            services.AddLogging();
        }

        private static void AddConfiguration(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json");

            IConfigurationRoot configuration = builder.Build();

            services.AddSingleton(configuration);
            services.AddOptions();
            services.Configure<LocationDiscoveryConfig>(configuration.GetSection("LocationDiscovery"));
        }

        private static void AddValidation(IServiceCollection services)
        {
            services.AddTransient<IValidator<Query>>((a) => { return new Validator(); });
        }
    }
}