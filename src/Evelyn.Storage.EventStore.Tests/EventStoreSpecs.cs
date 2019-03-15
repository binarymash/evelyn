namespace Evelyn.Storage.EventStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AutoFixture;
    using Core.WriteModel;
    using CQRSlite.Events;

    public abstract class EventStoreSpecs : IDisposable
    {
        protected EventStoreSpecs()
        {
            EnsureCoreAssemblyIsLoaded();
            BootstrapEmbeddedEventStore();
            DataFixture = new Fixture();
            EventsAddedToStore = new List<IEvent>();
        }

        protected Fixture DataFixture { get; }

        protected global::EventStore.Core.ClusterVNode Server { get; private set; }

        protected global::EventStore.ClientAPI.IEventStoreConnection ManagementConnection { get; private set; }

        protected List<IEvent> EventsAddedToStore { get; }

        public void Dispose()
        {
            TearDownEmbeddedEventStore();
        }

        private void EnsureCoreAssemblyIsLoaded()
        {
            System.Diagnostics.Trace.WriteLine($"Let's ensure that the core assembly is loaded by referencing {typeof(WriteModelAssemblyMarker).FullName}");
        }

        private void BootstrapEmbeddedEventStore()
        {
            Server = global::EventStore.ClientAPI.Embedded.EmbeddedVNodeBuilder
                .AsSingleNode()
                .RunInMemory()
                .RunProjections(global::EventStore.Common.Options.ProjectionType.All)
                .OnDefaultEndpoints()
                .StartStandardProjections()
                .AddExternalHttpPrefix("http://*:2113/")
                .Build();

            var startedEvent = new ManualResetEventSlim(false);
            Server.MainBus.Subscribe(
                new global::EventStore.Core.Bus.AdHocHandler<global::EventStore.Core.Messages.UserManagementMessage.UserManagementServiceInitialized>(m => startedEvent.Set()));

            Server.Start();

            if (!startedEvent.Wait(60000))
            {
                throw new TimeoutException("Embedded Event Store has not started in 60 seconds.");
            }

            var connectionSettings = global::EventStore.ClientAPI.ConnectionSettings
                .Create()
                .SetDefaultUserCredentials(new global::EventStore.ClientAPI.SystemData.UserCredentials("admin", "changeit"))
                .Build();

            ManagementConnection = global::EventStore.ClientAPI.Embedded.EmbeddedEventStoreConnection.Create(Server, connectionSettings);
            ManagementConnection.ConnectAsync().GetAwaiter().GetResult();
        }

        private void TearDownEmbeddedEventStore()
        {
            ManagementConnection?.Close();

            if (!Server.Stop(TimeSpan.FromSeconds(30), true, true))
            {
                throw new Exception("Failed to stop embedded eventstore server");
            }
        }

        protected class EventStoreConnectionFactory : IEventStoreConnectionFactory
        {
            public global::EventStore.ClientAPI.IEventStoreConnection Invoke()
            {
                var connectionSettings = global::EventStore.ClientAPI.ConnectionSettings
                    .Create()
                    .SetDefaultUserCredentials(new global::EventStore.ClientAPI.SystemData.UserCredentials("admin", "changeit"))
                    .Build();

                var uri = new Uri("tcp://127.0.0.1:1113");

                return global::EventStore.ClientAPI.EventStoreConnection.Create(connectionSettings, uri);
            }
        }
    }
}
