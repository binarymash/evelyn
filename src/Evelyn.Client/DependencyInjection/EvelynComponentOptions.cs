// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public abstract class EvelynComponentOptions
    {
        protected EvelynComponentOptions(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
