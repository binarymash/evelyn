// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class CatchUpSubscriptionPublisherOptions : EvelynComponentOptions
    {
        public CatchUpSubscriptionPublisherOptions(IServiceCollection services)
            : base(services)
        {
            PublishEvents = new EventPublisherOptions(services);
        }

        public EventPublisherOptions PublishEvents { get; }
    }
}
