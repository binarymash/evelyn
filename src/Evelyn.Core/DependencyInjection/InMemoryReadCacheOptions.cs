// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the in-memory read cache
    /// </summary>
    public class InMemoryReadCacheOptions : EvelynComponentOptions
    {
        public InMemoryReadCacheOptions(IServiceCollection services)
            : base(services)
        {
        }
    }
}
