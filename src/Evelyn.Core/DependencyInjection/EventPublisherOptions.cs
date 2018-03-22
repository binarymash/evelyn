// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures the event publisher
    /// </summary>
    public class EventPublisherOptions : EvelynComponentOptions
    {
        public EventPublisherOptions(IServiceCollection services)
            : base(services)
        {
        }
    }
}
