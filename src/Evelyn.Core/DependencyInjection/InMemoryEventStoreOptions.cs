// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class InMemoryEventStoreOptions : EvelynComponentOptions
    {
        public InMemoryEventStoreOptions(IServiceCollection services)
            : base(services)
        {
            WithEventPublisher = new InMemoryEventStorePublisherOptions(services);
        }

        /// <summary>
        /// Configures how we will publish events from the in-memory Event Store.
        /// </summary>
        /// <value>
        /// The event publisher options
        /// </value>
        public InMemoryEventStorePublisherOptions WithEventPublisher { get; }
    }
}