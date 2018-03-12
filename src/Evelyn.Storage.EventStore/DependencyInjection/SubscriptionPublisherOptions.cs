// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class SubscriptionPublisherOptions : EvelynComponentOptions
    {
        public SubscriptionPublisherOptions(IServiceCollection services)
            : base(services)
        {
            PublishEvents = new EventPublisherOptions(services);
        }

        public EventPublisherOptions PublishEvents { get; }
    }
}
