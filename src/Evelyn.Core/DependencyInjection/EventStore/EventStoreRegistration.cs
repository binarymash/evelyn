// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures how events will be stored
    /// </summary>
    public class EventStoreRegistration : EvelynComponentRegistration
    {
        public EventStoreRegistration(IServiceCollection services)
            : base(services)
        {
        }
    }
}
