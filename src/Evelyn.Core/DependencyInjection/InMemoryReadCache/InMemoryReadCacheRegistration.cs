// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class InMemoryReadCacheRegistration : EvelynComponentRegistration
    {
        public InMemoryReadCacheRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
