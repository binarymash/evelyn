// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Configures how events will be stored
    /// </summary>
    public class EventStoreOptions : EvelynComponentOptions
    {
        public EventStoreOptions(IServiceCollection services)
            : base(services)
        {
        }
    }
}
