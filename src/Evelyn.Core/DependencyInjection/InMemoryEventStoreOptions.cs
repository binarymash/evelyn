// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the in-memory event store
    /// </summary>
    public class InMemoryEventStoreOptions : EvelynComponentOptions
    {
        public InMemoryEventStoreOptions(IServiceCollection services)
            : base(services)
        {
            WithEventPublisher = new EventPublisherOptions(services);
        }

        /// <summary>
        /// Configures the event publisher options on the in-memory event store
        /// </summary>
        /// <value>
        /// The event publisher options
        /// </value>
        public EventPublisherOptions WithEventPublisher { get; }
    }
}
