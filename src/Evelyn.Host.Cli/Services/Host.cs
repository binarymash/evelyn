namespace Evelyn.Host.Cli.Services
{
    using Microsoft.Extensions.DependencyInjection;

    public static class Host
    {
        public static void AddHostApplication(this IServiceCollection services)
        {
            services.AddTransient<Application>();
        }
    }
}
