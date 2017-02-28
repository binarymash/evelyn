namespace Evelyn.Host.Cli.Services
{
    using System.IO;
    using Evelyn.Agent.Features.Locations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class Configuration
    {
        public static void AddConfiguration(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json");

            IConfigurationRoot configuration = builder.Build();

            services.AddSingleton(configuration);
            services.AddOptions();
            services.Configure<LocationDiscoveryConfig>(configuration.GetSection("LocationDiscovery"));
        }
    }
}
