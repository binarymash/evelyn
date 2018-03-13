namespace Evelyn.Storage.EventStore.Tests
{
    extern alias EmbeddedES;
    extern alias NetCoreES;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.WriteModel.Project.Events;
    using CQRSlite.Events;
    using FluentAssertions;
    using NSubstitute;
    using TestStack.BDDfy;
    using Xunit;
    using ProjectCreated = Core.WriteModel.Account.Events.ProjectCreated;

    public class SubscriptionPublisherSpecs : EventStoreSpecs
    {
        private readonly EventStoreImplementation _eventStore;
        private readonly SubscriptionPublisher _subscriptionPublisher;
        private readonly IList<IEvent> _expectedEvents;
        private readonly IList<IEvent> _publishedEvents;

        public SubscriptionPublisherSpecs()
        {
            _eventStore = new EventStoreImplementation(new EventStoreConnectionFactory());

            var eventPublisher = Substitute.For<IEventPublisher>();
            eventPublisher
                .WhenForAnyArgs(p => p.Publish(Arg.Any<IEvent>()))
                .Do(callInfo =>
                {
                    _publishedEvents.Add(callInfo.ArgAt<IEvent>(0));
                });

            _subscriptionPublisher = new SubscriptionPublisher(new EventStoreConnectionFactory(), eventPublisher);
            _expectedEvents = new List<IEvent>();
            _publishedEvents = new List<IEvent>();
        }

        [Fact]
        public void EventsPublishedOnStartUp()
        {
            this.Given(_ => GivenSomeEventsOnAnAggregate())
                .And(_ => GivenSomeEventsOnAnotherAggregate())
                .When(_ => WhenTheSubscriptionPublisherIsStarted())
                .Then(_ => ThenAllTheEventsArePublishedWithin2Seconds())
                .BDDfy();
        }

        [Fact]
        public void NewEventsArePublished()
        {
            this.Given(_ => GivenTheSubscriptionPublisherIsStarted())
                .When(_ => WhenNewEventsAreAdded())
                .Then(_ => ThenTheNewEventsArePublishedWithin2Seconds())
                .When(_ => WhenMoreNewEventsAreAdded())
                .Then(_ => ThenTheNewEventsArePublishedWithin2Seconds())
                .BDDfy();
        }

        private async Task GivenSomeEventsOnAnAggregate()
        {
            var aggregateId = DataFixture.Create<Guid>();

            var events = new List<IEvent>
            {
                CreateEvent<ProjectCreated>(aggregateId, 0),
                CreateEvent<ToggleAdded>(aggregateId, 1),
                CreateEvent<EnvironmentAdded>(aggregateId, 2),
                CreateEvent<ToggleAdded>(aggregateId, 3),
            };

            await _eventStore.Save(events);

            foreach (var @event in events)
            {
                _expectedEvents.Add(@event);
            }
        }

        private async Task GivenSomeEventsOnAnotherAggregate()
        {
            var aggregateId = DataFixture.Create<Guid>();

            var events = new List<IEvent>
            {
                CreateEvent<ToggleAdded>(aggregateId, 0),
                CreateEvent<EnvironmentAdded>(aggregateId, 1),
                CreateEvent<EnvironmentAdded>(aggregateId, 2),
            };

            await _eventStore.Save(events);

            foreach (var @event in events)
            {
                _expectedEvents.Add(@event);
            }
        }

        private T CreateEvent<T>(Guid aggregateId, int? version = null)
            where T : IEvent
        {
            var @event = DataFixture.Build<T>().With(e => e.Id, aggregateId).Create();
            @event.Version = version ?? EventsAddedToStore.Count;
            @event.TimeStamp = DateTimeOffset.UtcNow;
            return @event;
        }

        private Task GivenTheSubscriptionPublisherIsStarted()
        {
            return StartPublisher();
        }

        private Task WhenTheSubscriptionPublisherIsStarted()
        {
            return StartPublisher();
        }

        private async Task StartPublisher()
        {
            await _subscriptionPublisher.StartAsync(default(CancellationToken));
        }

        private async Task WhenNewEventsAreAdded()
        {
            var aggregateId = DataFixture.Create<Guid>();

            var events = new List<IEvent>
            {
                CreateEvent<EnvironmentAdded>(aggregateId, 0),
                CreateEvent<ToggleAdded>(aggregateId, 1),
                CreateEvent<ProjectCreated>(aggregateId, 2)
            };

            await _eventStore.Save(events);

            foreach (var @event in events)
            {
                _expectedEvents.Add(@event);
            }
        }

        private async Task WhenMoreNewEventsAreAdded()
        {
            _publishedEvents.Clear();
            _expectedEvents.Clear();

            var aggregateId = DataFixture.Create<Guid>();

            var events = new List<IEvent>
            {
                CreateEvent<ToggleAdded>(aggregateId, 0),
                CreateEvent<ProjectCreated>(aggregateId, 1),
                CreateEvent<EnvironmentAdded>(aggregateId, 2)
            };

            await _eventStore.Save(events);

            foreach (var @event in events)
            {
                _expectedEvents.Add(@event);
            }
        }

        private void ThenAllTheEventsArePublishedWithin2Seconds()
        {
            ThenAllTheExpectedEventsArePublishedWithin2Seconds();
        }

        private void ThenTheNewEventsArePublishedWithin2Seconds()
        {
            ThenAllTheExpectedEventsArePublishedWithin2Seconds();
        }

        private void ThenAllTheExpectedEventsArePublishedWithin2Seconds()
        {
            var allEventsPublished = false;
            var startTime = DateTime.UtcNow;
            var maxEndTime = startTime.Add(TimeSpan.FromSeconds(2));

            while (!allEventsPublished && NotYetReached(maxEndTime))
            {
                allEventsPublished = _publishedEvents.Count == _expectedEvents.Count;
                Task.Delay(TimeSpan.FromMilliseconds(50));
            }

            allEventsPublished.Should().BeTrue();

            for (var idx = 0; idx < _expectedEvents.Count; idx++)
            {
                _publishedEvents[idx].Should().BeEquivalentTo(_expectedEvents[idx]);
            }
        }

        private bool NotYetReached(DateTime maxEndTime)
        {
            return DateTime.UtcNow < maxEndTime;
        }
    }
}
