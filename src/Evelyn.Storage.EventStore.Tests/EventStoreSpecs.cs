using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Evelyn.Storage.EventStore.Tests
{
    extern alias EmbeddedES;
    extern alias NetCoreES;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AutoFixture;
    using Core.WriteModel;
    using CQRSlite.Events;
    using EmbeddedES::EventStore.Common.Options;
    using EmbeddedES::EventStore.Core.Bus;
    using EmbeddedES::EventStore.Core.Messages;
    using NetCoreES::EventStore.ClientAPI.SystemData;

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

        protected EmbeddedES::EventStore.Core.ClusterVNode Server { get; private set; }

        protected EmbeddedES::EventStore.ClientAPI.IEventStoreConnection ManagementConnection { get; private set; }

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
            Server = EmbeddedES::EventStore.ClientAPI.Embedded.EmbeddedVNodeBuilder
                .AsSingleNode()
                .RunInMemory()
                .RunProjections(ProjectionType.All)
                .OnDefaultEndpoints()
                .StartStandardProjections()
                .AddExternalHttpPrefix("http://*:2113/")
                .Build();

            var startedEvent = new ManualResetEventSlim(false);
            Server.MainBus.Subscribe(
                new AdHocHandler<UserManagementMessage.UserManagementServiceInitialized>(m => startedEvent.Set()));

            Server.Start();

            if (!startedEvent.Wait(60000))
            {
                throw new TimeoutException("Embedded Event Store has not started in 60 seconds.");
            }

            var connectionSettings = EmbeddedES::EventStore.ClientAPI.ConnectionSettings
                .Create()
                .SetDefaultUserCredentials(new EmbeddedES::EventStore.ClientAPI.SystemData.UserCredentials("admin", "changeit"))
                .Build();

            ManagementConnection = EmbeddedES::EventStore.ClientAPI.Embedded.EmbeddedEventStoreConnection.Create(Server, connectionSettings);
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
            public NetCoreES::EventStore.ClientAPI.IEventStoreConnection Invoke()
            {
                var connectionSettings = NetCoreES::EventStore.ClientAPI.ConnectionSettings
                    .Create()
                    .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"))
                    .Build();

                var uri = new Uri("tcp://127.0.0.1:1113");

                return NetCoreES::EventStore.ClientAPI.EventStoreConnection.Create(connectionSettings, uri);
            }
        }
    }
}
