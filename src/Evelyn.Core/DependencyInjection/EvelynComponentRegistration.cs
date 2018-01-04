// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public abstract class EvelynComponentRegistration
    {
        protected EvelynComponentRegistration(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
