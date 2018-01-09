namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the write model options
    /// </summary>
    public class WriteModelOptions : EvelynComponentOptions
    {
        public WriteModelOptions(IServiceCollection services)
            : base(services)
        {
            WithEventStore = new EventStoreOptions(services);
        }

        /// <summary>
        /// Configures the event store options
        /// </summary>
        /// <value>
        /// The Event store options
        /// </value>
        public EventStoreOptions WithEventStore { get; }
    }
}
