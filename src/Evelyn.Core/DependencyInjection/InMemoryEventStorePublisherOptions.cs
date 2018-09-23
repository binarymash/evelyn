// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class InMemoryEventStorePublisherOptions : EvelynComponentOptions
    {
        public InMemoryEventStorePublisherOptions(IServiceCollection services)
            : base(services)
        {
        }
    }
}
