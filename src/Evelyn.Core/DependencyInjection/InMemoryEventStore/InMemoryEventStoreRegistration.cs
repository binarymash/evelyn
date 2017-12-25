// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class InMemoryEventStoreRegistration : EvelynComponentRegistration
    {
        public InMemoryEventStoreRegistration(IServiceCollection services)
            : base(services)
        {
            WithEventPublisher = new EventPublisherRegistration(services);
        }

        public EventPublisherRegistration WithEventPublisher { get; }
    }
}
