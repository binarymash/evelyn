// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class EventStoreCatchUpSubscriptionPublisherOptions : EvelynComponentOptions
    {
        public EventStoreCatchUpSubscriptionPublisherOptions(IServiceCollection services)
            : base(services)
        {
            PublishEvents = new EventPublisherOptions(services);
        }

        public EventPublisherOptions PublishEvents { get; }
    }
}
