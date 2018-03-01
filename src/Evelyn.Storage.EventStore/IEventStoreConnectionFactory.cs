namespace Evelyn.Storage.EventStore
{
    using global::EventStore.ClientAPI;

    public interface IEventStoreConnectionFactory
    {
        IEventStoreConnection Invoke();
    }
}
