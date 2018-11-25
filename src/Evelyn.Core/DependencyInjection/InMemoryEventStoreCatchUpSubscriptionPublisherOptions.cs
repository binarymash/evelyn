// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class InMemoryEventStoreCatchUpSubscriptionPublisherOptions : EvelynComponentOptions
    {
        public InMemoryEventStoreCatchUpSubscriptionPublisherOptions(IServiceCollection services)
            : base(services)
        {
            PublishEvents = new EventPublisherOptions(services);
        }

        public EventPublisherOptions PublishEvents { get; }
    }
}
