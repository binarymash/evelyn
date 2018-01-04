namespace Evelyn.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using AutoFixture;
    using CQRSlite.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;

    public class InMemoryEventStoreSpecs
    {
        private Fixture _fixture;
        private IEventPublisher _publisher;
        private InMemoryEventStore _eventStore;
        private IList<IEvent> _eventsToStore;
        private Guid _existingAggregateRootId;
        private IEvent _eventToSave1;
        private IEvent _eventToSave2;
        private Guid _aggregateRootIdForGet;
        private IEnumerable<IEvent> _retrievedEvents;

        public InMemoryEventStoreSpecs()
        {
            _fixture = new Fixture();
            _publisher = Substitute.For<IEventPublisher>();
            _eventStore = new InMemoryEventStore(_publisher);
            _eventsToStore = new List<IEvent>();
        }

        [Fact]
        public void StoringNoEvents()
        {
            this.Given(_ => GivenThereAreNoEventsToStore())
                .When(_ => WhenSaved())
                .Then(_ => ThenNoEventsArePublished())
                .BDDfy();
        }

        [Fact]
        public void StoringAnEventForAnAggregateRoot()
        {
            this.Given(_ => GivenAnEventOnAnAggregateRoot())
                .When(_ => WhenSaved())
                .Then(_ => ThenTheEventIsPublished())
                .BDDfy();
        }

        [Fact]
        public void StoringMultipleEventsOnTheSameAggregateRoot()
        {
            this.Given(_ => GivenWeAlreadyHaveAlreadySavedAnEventOnAnAggregateRoot())
                .Given(_ => GiveWeHaveAnotherEventOnTheSameAggregateRoot())
                .When(_ => WhenSaved())
                .Then(_ => ThenTheEventIsPublished())
                .BDDfy();
        }

        [Fact]
        public void StoringEventsOnMultipleAggregateRoots()
        {
            this.Given(_ => GivenAnEventOnAnAggregateRoot())
                .And(_ => GivenAnotherEventOnAnotherAggregateRoot())
                .When(_ => WhenSaved())
                .Then(_ => ThenBothEventsArePublished())
                .BDDfy();
        }

        [Fact]
        public void GettingForAnAggregateRootThatDoesntExist()
        {
            this.Given(_ => GivenAnAggregateRootThatDoesNotExist())
                .When(_ => WhenWeGetAllEventsForTheAggregateRoot())
                .Then(_ => ThenNoEventsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void GettingAllEventsForAnAggregateRoot()
        {
            this.Given(_ => GivenThatThereAreMultipleEventsOnAnAggregateRoot())
                .When(_ => WhenWeGetAllEventsForTheAggregateRoot())
                .Then(_ => ThenAllTheEventsAreReturned())
                .BDDfy();
        }

        [Fact]
        public void GettingSpecificEventsForAnAggregateRoot()
        {
            this.Given(_ => GivenThatThereAreMultipleEventsOnAnAggregateRoot())
                .When(_ => WhenWeGetEventsAfterVersion1ForTheAggregateRoot())
                .Then(_ => ThenAllEventsAfterVersion1AreReturned())
                .BDDfy();
        }

        private void GivenAnAggregateRootThatDoesNotExist()
        {
            _aggregateRootIdForGet = _fixture.Create<Guid>();
        }

        private void GivenThereAreNoEventsToStore()
        {
        }

        private void GivenAnEventOnAnAggregateRoot()
        {
            _eventToSave1 = _fixture.Create<MyEvent>();
            _eventsToStore.Add(_eventToSave1);
        }

        private void GivenWeAlreadyHaveAlreadySavedAnEventOnAnAggregateRoot()
        {
            var ev = _fixture.Create<MyEvent>();
            _existingAggregateRootId = ev.Id;
            _eventStore.Save(new[] { ev }).GetAwaiter().GetResult();
        }

        private void GiveWeHaveAnotherEventOnTheSameAggregateRoot()
        {
            _eventToSave1 = _fixture.Create<MyEvent>();
            _eventToSave1.Id = _existingAggregateRootId;
            _eventsToStore.Add(_eventToSave1);
        }

        private void GivenAnotherEventOnAnotherAggregateRoot()
        {
            _eventToSave2 = _fixture.Create<MyEvent>();
            _eventsToStore.Add(_eventToSave2);
        }

        private void GivenThatThereAreMultipleEventsOnAnAggregateRoot()
        {
            _eventToSave1 = _fixture.Create<MyEvent>();
            _eventToSave1.Version = 1;
            _eventsToStore.Add(_eventToSave1);

            _eventToSave2 = _fixture.Create<MyEvent>();
            _eventToSave2.Version = 2;
            _eventToSave2.Id = _eventToSave1.Id;
            _eventsToStore.Add(_eventToSave2);

            _eventStore.Save(_eventsToStore).GetAwaiter().GetResult();

            _aggregateRootIdForGet = _eventToSave1.Id;
        }

        private void WhenSaved()
        {
            _eventStore.Save(_eventsToStore).GetAwaiter().GetResult();
        }

        private void WhenWeGetAllEventsForTheAggregateRoot()
        {
            _retrievedEvents = _eventStore.Get(_aggregateRootIdForGet, 0).GetAwaiter().GetResult();
        }

        private void WhenWeGetEventsAfterVersion1ForTheAggregateRoot()
        {
            _retrievedEvents = _eventStore.Get(_aggregateRootIdForGet, 1).GetAwaiter().GetResult();
        }

        private void ThenNoEventsArePublished()
        {
            _publisher.DidNotReceive().Publish(Arg.Any<IEvent>(), Arg.Any<CancellationToken>());
        }

        private void ThenTheEventIsPublished()
        {
            _publisher.Received().Publish(_eventToSave1);
        }

        private void ThenBothEventsArePublished()
        {
            _publisher.Received().Publish(_eventToSave1);
            _publisher.Received().Publish(_eventToSave2);
        }

        private void ThenNoEventsAreReturned()
        {
            _retrievedEvents.Count().Should().Be(0);
        }

        private void ThenAllTheEventsAreReturned()
        {
            _retrievedEvents.Count().Should().Be(2);
            _retrievedEvents.Should().Contain(_eventToSave1);
            _retrievedEvents.Should().Contain(_eventToSave2);
        }

        private void ThenAllEventsAfterVersion1AreReturned()
        {
            _retrievedEvents.Count().Should().Be(1);
            _retrievedEvents.Should().Contain(_eventToSave2);
        }

        private class MyEvent : IEvent
        {
            public MyEvent(Guid id, int version, DateTimeOffset timeStamp)
            {
                Id = id;
                Version = version;
                TimeStamp = timeStamp;
            }

            public Guid Id { get; set; }

            public int Version { get; set; }

            public DateTimeOffset TimeStamp { get; set; }
        }
    }
}
