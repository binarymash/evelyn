// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class WriteModelOptions : EvelynComponentOptions
    {
        public WriteModelOptions(IServiceCollection services)
            : base(services)
        {
            WithEventStore = new EventStoreOptions(services);
        }

        /// <summary>
        /// Configures how we will store the events that are created when a command is processed.
        /// </summary>
        /// <value>
        /// The Event Store options
        /// </value>
        public EventStoreOptions WithEventStore { get; }
    }
}
