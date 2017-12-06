namespace Microsoft.Extensions.DependencyInjection
{
    public class RehydrateFromEventStoreRegistration : EvelynComponentRegistration
    {
        public RehydrateFromEventStoreRegistration(IServiceCollection services)
            : base(services)
        {
            FromEventStore = new EventStoreRegistration(services);
        }

        public EventStoreRegistration FromEventStore { get; }
    }
}