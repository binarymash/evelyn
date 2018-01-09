namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the write model options
    /// </summary>
    public class WriteModelRegistration : EvelynComponentRegistration
    {
        public WriteModelRegistration(IServiceCollection services)
            : base(services)
        {
            WithEventStore = new EventStoreRegistration(services);
        }

        /// <summary>
        /// Configures the event store options
        /// </summary>
        /// <value>
        /// The Event store options
        /// </value>
        public EventStoreRegistration WithEventStore { get; }
    }
}
