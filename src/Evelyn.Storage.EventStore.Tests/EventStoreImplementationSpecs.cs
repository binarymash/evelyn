namespace Evelyn.Storage.EventStore.Tests
{
    extern alias EmbeddedES;
    extern alias NetCoreES;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.ReadModel.Events;
    using CQRSlite.Events;
    using Evelyn.Storage.EventStore;
    using FluentAssertions;
    using NetCoreES::EventStore.ClientAPI;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStoreImplementationSpecs : IDisposable
    {
        private readonly Fixture _fixture;
        private readonly NetCoreES::EventStore.ClientAPI.IEventStoreConnection _connection;
        private readonly EventStoreImplementation _eventStore;
        private readonly Guid _aggregateId;
        private readonly List<IEvent> _eventsAddedToStore;

        private List<IEvent> _returnedEvents;
        private EmbeddedES::EventStore.Core.ClusterVNode _server;
        private EmbeddedES::EventStore.ClientAPI.IEventStoreConnection _managementConnection;

        public EventStoreImplementationSpecs()
        {
            BootstrapEmbeddedEventStore();
            _eventStore = new EventStoreImplementation(new EventStoreConnectionFactory());
            _fixture = new Fixture();
            _aggregateId = _fixture.Create<Guid>();
            _eventsAddedToStore = new List<IEvent>();
        }

        [Fact]
        public void AggregateHasNoEvents()
        {
            this.Given(_ => GivenAnAggregateHasNoEvents())
                .When(_ => WhenWeGetAllEventsForTheAggregate())
                .Then(_ => ThenNoEventsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void AggregateHasEvents()
        {
            this.Given(_ => GivenWeHave3EventsForAnAggregate())
                .When(_ => WhenWeSaveTheseEventsToTheStore())
                .And(_ => WhenWeGetAllEventsForTheAggregate())
                .Then(_ => ThenAllEventsAreReturned())
                .When(_ => WhenWeGetAllEventsFromVersion0())
                .Then(_ => ThenTheFinal2EventsAreReturned())
                .When(_ => WhenWeGetAllEventsFromVersion1())
                .Then(_ => ThenTheFinalEventIsReturned())
                .When(_ => WhenWeGetAllEventsFromVersion2())
                .Then(_ => ThenNoEventsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void AggregateHasMoreEventsThanTheSliceSize()
        {
            this.Given(_ => GivenWeHave300EventsForAnAggregate())
                .When(_ => WhenWeSaveTheseEventsToTheStore())
                .And(_ => WhenWeGetAllEventsForTheAggregate())
                .Then(_ => ThenAllEventsAreReturned())
                .When(_ => WhenWeGetAllEventsFromVersion0())
                .Then(_ => ThenTheFinal299EventsAreReturned())
                .When(_ => WhenWeGetAllEventsFromVersion298())
                .Then(_ => ThenTheFinalEventIsReturned())
                .When(_ => WhenWeGetAllEventsFromVersion299())
                .Then(_ => ThenNoEventsAreReturned())
                .BDDfy();
        }

        public void Dispose()
        {
            TearDownEmbeddedEventStore();
        }

        private void BootstrapEmbeddedEventStore()
        {
            _server = EmbeddedES::EventStore.ClientAPI.Embedded.EmbeddedVNodeBuilder
                .AsSingleNode()
                .RunInMemory()
                .OnDefaultEndpoints()
                .Build();

            var isNodeMaster = false;

            _server.NodeStatusChanged += (sender, args) => { isNodeMaster = args.NewVNodeState == EmbeddedES::EventStore.Core.Data.VNodeState.Master; };

            _server.Start();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!isNodeMaster)
            {
                if (stopwatch.Elapsed.Seconds > 20)
                {
                    throw new InvalidOperationException("Waited too long (20 seconds) for EventStore node to become master.");
                }

                Thread.Sleep(1000);
            }

            stopwatch.Stop();

            _managementConnection = EmbeddedES::EventStore.ClientAPI.Embedded.EmbeddedEventStoreConnection.Create(_server);
            _managementConnection.ConnectAsync().GetAwaiter().GetResult();
        }

        private void TearDownEmbeddedEventStore()
        {
            _managementConnection?.Dispose();
        }

        private void GivenAnAggregateHasNoEvents()
        {
        }

        private void GivenWeHave3EventsForAnAggregate()
        {
            GivenAggregateEvent(_fixture.Build<ApplicationCreated>().With(e => e.Id, _aggregateId).Create());
            GivenAggregateEvent(_fixture.Build<EnvironmentAdded>().With(e => e.Id, _aggregateId).Create());
            GivenAggregateEvent(_fixture.Build<ToggleAdded>().With(e => e.Id, _aggregateId).Create());
        }

        private void GivenWeHave300EventsForAnAggregate()
        {
            GivenAggregateEvent(_fixture.Build<ApplicationCreated>().With(e => e.Id, _aggregateId).Create());
            GivenAggregateEvents(_fixture.Build<ToggleAdded>().With(e => e.Id, _aggregateId).CreateMany(299));
        }

        private void GivenAggregateEvent(IEvent @event)
        {
            @event.Version = _eventsAddedToStore.Count;
            @event.TimeStamp = DateTimeOffset.UtcNow;
            _eventsAddedToStore.Add(@event);
        }

        private void GivenAggregateEvents(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                @event.Version = _eventsAddedToStore.Count;
                @event.TimeStamp = DateTimeOffset.UtcNow;
                _eventsAddedToStore.Add(@event);
            }
        }

        private async Task WhenWeSaveTheseEventsToTheStore()
        {
            await _eventStore.Save(_eventsAddedToStore);
        }

        private async Task WhenWeGetAllEventsForTheAggregate()
        {
            await WhenWeGetAllEventsFromVersion(-1);
        }

        private async Task WhenWeGetAllEventsFromVersion0()
        {
            await WhenWeGetAllEventsFromVersion(0);
        }

        private async Task WhenWeGetAllEventsFromVersion1()
        {
            await WhenWeGetAllEventsFromVersion(1);
        }

        private async Task WhenWeGetAllEventsFromVersion2()
        {
            await WhenWeGetAllEventsFromVersion(2);
        }

        private async Task WhenWeGetAllEventsFromVersion298()
        {
            await WhenWeGetAllEventsFromVersion(298);
        }

        private async Task WhenWeGetAllEventsFromVersion299()
        {
            await WhenWeGetAllEventsFromVersion(299);
        }

        private async Task WhenWeGetAllEventsFromVersion(int fromVersion)
        {
            _returnedEvents = (await _eventStore.Get(_aggregateId, fromVersion)).ToList();
        }

        private void ThenNoEventsAreReturned()
        {
            _returnedEvents.Should().HaveCount(0);
        }

        private void ThenAllEventsAreReturned()
        {
            ThenWeAreReturnedEventsFromPosition(0);
        }

        private void ThenTheFinal2EventsAreReturned()
        {
            ThenWeAreReturnedEventsFromPosition(_eventsAddedToStore.Count - 2);
        }

        private void ThenTheFinal299EventsAreReturned()
        {
            ThenWeAreReturnedEventsFromPosition(_eventsAddedToStore.Count - 299);
        }

        private void ThenTheFinalEventIsReturned()
        {
            ThenWeAreReturnedEventsFromPosition(_eventsAddedToStore.Count - 1);
        }

        private void ThenWeAreReturnedEventsFromPosition(int startPosition)
        {
            var expectedNumberOfResults = _eventsAddedToStore.Count - startPosition;
            if (expectedNumberOfResults < 0)
            {
                expectedNumberOfResults = 0;
            }

            _returnedEvents.Should().HaveCount(expectedNumberOfResults);

            var addedEventIndex = startPosition;
            for (var returnedEventIndex = 0; returnedEventIndex < _returnedEvents.Count; returnedEventIndex++)
            {
                ThenEventsMatch(_returnedEvents[returnedEventIndex], _eventsAddedToStore[addedEventIndex]);
                addedEventIndex++;
            }
        }

        private void ThenEventsMatch(IEvent event1, IEvent event2)
        {
            event1.Should().BeEquivalentTo(event2);
        }

        private class EventStoreConnectionFactory : IEventStoreConnectionFactory
        {
            public IEventStoreConnection Invoke()
            {
                var connectionSettings = NetCoreES::EventStore.ClientAPI.ConnectionSettings
                    .Create()
                    .Build();

                var uri = new Uri("tcp://127.0.0.1:1113");

                return NetCoreES::EventStore.ClientAPI.EventStoreConnection.Create(connectionSettings, uri);
            }
        }
    }
}
