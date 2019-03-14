namespace Evelyn.Storage.EventStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Events;
    using Evelyn.Storage.EventStore;
    using FluentAssertions;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStoreImplementationSpecs : EventStoreSpecs
    {
        private readonly EventStoreImplementation _eventStore;
        private readonly Guid _aggregateId;

        private List<IEvent> _returnedEvents;

        public EventStoreImplementationSpecs()
        {
            _eventStore = new EventStoreImplementation(new EventStoreConnectionFactory());
            _aggregateId = DataFixture.Create<Guid>();
        }

        [Fact]
        public void GetAggregateThatDoesntHaveEvents()
        {
            this.Given(_ => GivenAnAggregateHasNoEvents())
                .When(_ => WhenWeGetAllEventsForTheAggregate())
                .Then(_ => ThenNoEventsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void SetAndGetAggregateEvents()
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
        public void SetAndGetAggregateEventsThatExceedSliceSize()
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

        [Fact]
        public void EventStreamName()
        {
            this.Given(_ => GivenWeHave3EventsForAnAggregate())
                .When(_ => WhenWeSaveTheseEventsToTheStore())
                .Then(_ => ThenTheStoredEventStreamIsNamedByAggregateRootIdWithEvelynPrefix())
                .BDDfy();
        }

        [Fact]
        public void EventTypePersistence()
        {
            this.Given(_ => GivenWeHave3EventsForAnAggregate())
                .When(_ => WhenWeSaveTheseEventsToTheStore())
                .And(_ => ThenTheStoredEventTypesAreTheFullNameOfTheEvent())
                .BDDfy();
        }

        [Fact]
        public void EventIDPersistence()
        {
            this.Given(_ => GivenWeHave3EventsForAnAggregate())
                .When(_ => WhenWeSaveTheseEventsToTheStore())
                .And(_ => ThenTheStoredEventsAllHaveDifferentIDs())
                .BDDfy();
        }

        private void GivenAnAggregateHasNoEvents()
        {
        }

        private void GivenWeHave3EventsForAnAggregate()
        {
            GivenAggregateEvent(DataFixture.Build<ProjectCreated>().With(e => e.Id, _aggregateId).Create());
            GivenAggregateEvent(DataFixture.Build<EnvironmentAdded>().With(e => e.Id, _aggregateId).Create());
            GivenAggregateEvent(DataFixture.Build<ToggleAdded>().With(e => e.Id, _aggregateId).Create());
        }

        private void GivenWeHave300EventsForAnAggregate()
        {
            GivenAggregateEvent(DataFixture.Build<ProjectCreated>().With(e => e.Id, _aggregateId).Create());
            GivenAggregateEvents(DataFixture.Build<ToggleAdded>().With(e => e.Id, _aggregateId).CreateMany(299));
        }

        private void GivenAggregateEvent(IEvent @event, int? version = null)
        {
            @event.Version = version ?? EventsAddedToStore.Count;
            @event.TimeStamp = DateTimeOffset.UtcNow;
            EventsAddedToStore.Add(@event);
        }

        private void GivenAggregateEvents(IEnumerable<IEvent> events, int? version = null)
        {
            foreach (var @event in events)
            {
                GivenAggregateEvent(@event, version);
            }
        }

        private async Task WhenWeSaveTheseEventsToTheStore()
        {
            await _eventStore.Save(EventsAddedToStore);
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
            ThenWeAreReturnedEventsFromPosition(EventsAddedToStore.Count - 2);
        }

        private void ThenTheFinal299EventsAreReturned()
        {
            ThenWeAreReturnedEventsFromPosition(EventsAddedToStore.Count - 299);
        }

        private void ThenTheFinalEventIsReturned()
        {
            ThenWeAreReturnedEventsFromPosition(EventsAddedToStore.Count - 1);
        }

        private void ThenWeAreReturnedEventsFromPosition(int startPosition)
        {
            var expectedNumberOfResults = EventsAddedToStore.Count - startPosition;
            if (expectedNumberOfResults < 0)
            {
                expectedNumberOfResults = 0;
            }

            _returnedEvents.Should().HaveCount(expectedNumberOfResults);

            var addedEventIndex = startPosition;

            for (var returnedEventIndex = 0; returnedEventIndex < _returnedEvents.Count; returnedEventIndex++)
            {
                ThenEventsMatch(_returnedEvents[returnedEventIndex], EventsAddedToStore[addedEventIndex]);
                addedEventIndex++;
            }
        }

        private void ThenEventsMatch(IEvent event1, IEvent event2)
        {
            event1.Should().BeEquivalentTo(event2);
        }

        private async Task ThenTheStoredEventStreamIsNamedByAggregateRootIdWithEvelynPrefix()
        {
            var expectedStreamName = $"evelyn-{_aggregateId}";
            var result = await ManagementConnection.ReadStreamEventsForwardAsync(expectedStreamName, 0, 2000, false);
            result.Events.Should().HaveCount(EventsAddedToStore.Count);
        }

        private async Task<global::EventStore.ClientAPI.StreamEventsSlice> GetProjectAggregateRootStream(Guid id)
        {
            var expectedStreamName = $"evelyn-{_aggregateId}";
            return await ManagementConnection.ReadStreamEventsForwardAsync(expectedStreamName, 0, 2000, false);
        }

        private async Task ThenTheStoredEventTypesAreTheFullNameOfTheEvent()
        {
            var events = (await GetProjectAggregateRootStream(_aggregateId)).Events;
            for (var i = 0; i < EventsAddedToStore.Count; i++)
            {
                ThenTheStoredEventTypeIsTheFullNameOfTheEvent(events[i].Event, EventsAddedToStore[i].GetType());
            }
        }

        private void ThenTheStoredEventTypeIsTheFullNameOfTheEvent(global::EventStore.ClientAPI.RecordedEvent @event, Type type)
        {
            @event.EventType.Should().Be(type.FullName);
        }

        private async Task ThenTheStoredEventsAllHaveDifferentIDs()
        {
            var events = (await GetProjectAggregateRootStream(_aggregateId)).Events;
            events.Select(e => e.Event.EventId).Should().OnlyHaveUniqueItems();
        }
    }
}
