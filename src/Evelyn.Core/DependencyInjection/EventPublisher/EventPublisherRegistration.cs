// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the event publisher
    /// </summary>
    public class EventPublisherRegistration : EvelynComponentRegistration
    {
        public EventPublisherRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
