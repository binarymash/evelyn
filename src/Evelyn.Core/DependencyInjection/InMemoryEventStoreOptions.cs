// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class InMemoryEventStoreOptions : EvelynComponentOptions
    {
        public InMemoryEventStoreOptions(IServiceCollection services)
            : base(services)
        {
            WithEventPublisher = new EventPublisherOptions(services);
        }

        /// <summary>
        /// Configures how we will publish events from the in-memory event store.
        /// </summary>
        /// <value>
        /// The event publisher options
        /// </value>
        public EventPublisherOptions WithEventPublisher { get; }
    }
}
