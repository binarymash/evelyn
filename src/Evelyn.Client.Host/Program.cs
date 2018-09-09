namespace Evelyn.Client.Host
{
    using System;
    using System.Threading;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Initialising...");

            var startup = new Startup();
            var services = new ServiceCollection();

            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            startup.OnStartup(serviceProvider);
        }
    }
}
