// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public class EventPublisherRegistration : EvelynComponentRegistration
    {
        public EventPublisherRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
