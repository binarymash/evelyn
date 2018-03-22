// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Storage.EventStore;

    public class EventStoreDotOrgOptions : EvelynComponentOptions
    {
        public EventStoreDotOrgOptions(IServiceCollection services)
            : base(services)
        {
            WithEventPublisher = new EventStorePublisherOptions(services);
        }

        public IEventStoreConnectionFactory ConnectionFactory { get; set; }

        /// <summary>
        /// Configures how we will publish events from Event Store.
        /// </summary>
        /// <value>
        /// The Event Store publisher options
        /// </value>
        public EventStorePublisherOptions WithEventPublisher { get; }
    }
}
