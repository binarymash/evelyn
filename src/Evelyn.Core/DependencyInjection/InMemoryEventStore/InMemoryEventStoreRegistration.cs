// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the in-memory event store
    /// </summary>
    public class InMemoryEventStoreRegistration : EvelynComponentRegistration
    {
        public InMemoryEventStoreRegistration(IServiceCollection services)
            : base(services)
        {
            WithEventPublisher = new EventPublisherRegistration(services);
        }

        /// <summary>
        /// Configures the event publisher options on the in-memory event store
        /// </summary>
        /// <value>
        /// The event publisher options
        /// </value>
        public EventPublisherRegistration WithEventPublisher { get; }
    }
}
