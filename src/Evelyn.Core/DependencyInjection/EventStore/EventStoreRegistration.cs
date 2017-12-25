// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class EventStoreRegistration : EvelynComponentRegistration
    {
        public EventStoreRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
