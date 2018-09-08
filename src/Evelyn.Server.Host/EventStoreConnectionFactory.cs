namespace Evelyn.Server.Host
{
    using System;
    using EventStore.ClientAPI;
    using Storage.EventStore;

    public class EventStoreConnectionFactory : IEventStoreConnectionFactory
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly Uri _uri;
        private readonly string _connectionName;

        public EventStoreConnectionFactory(string uri, string connectionName = "Evelyn.Server.Host")
        {
            _connectionSettings = ConnectionSettings.Create()
                ////.EnableVerboseLogging()
                .UseConsoleLogger()
                .KeepReconnecting()
                .Build();
            _uri = new Uri(uri);
            _connectionName = connectionName;
        }

        public IEventStoreConnection Invoke()
        {
            return EventStoreConnection.Create(_connectionSettings, _uri, _connectionName);
        }
    }
}