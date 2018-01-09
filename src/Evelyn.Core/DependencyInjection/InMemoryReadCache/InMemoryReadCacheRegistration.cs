// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the in-memory read cache
    /// </summary>
    public class InMemoryReadCacheRegistration : EvelynComponentRegistration
    {
        public InMemoryReadCacheRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
