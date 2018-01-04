namespace Microsoft.Extensions.DependencyInjection
{
    public class WriteModelRegistration : EvelynComponentRegistration
    {
        public WriteModelRegistration(IServiceCollection services)
            : base(services)
        {
            WithEventStore = new EventStoreRegistration(services);
        }

        public EventStoreRegistration WithEventStore { get; }
    }
}
