namespace Microsoft.Extensions.DependencyInjection
{
    public abstract class EvelynComponentRegistration
    {
        public EvelynComponentRegistration(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
