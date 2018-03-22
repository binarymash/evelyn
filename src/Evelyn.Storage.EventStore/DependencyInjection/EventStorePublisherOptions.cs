// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class EventStorePublisherOptions : EvelynComponentOptions
    {
        public EventStorePublisherOptions(IServiceCollection services)
            : base(services)
        {
        }
    }
}
