// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using Evelyn.Storage.EventStore;

    public class EventStoreDotOrgOptions
    {
        public IEventStoreConnectionFactory ConnectionFactory { get; set; }
    }
}
