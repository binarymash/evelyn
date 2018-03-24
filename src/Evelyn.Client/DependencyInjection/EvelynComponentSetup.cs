// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public abstract class EvelynComponentSetup
    {
        protected EvelynComponentSetup(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
