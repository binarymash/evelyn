namespace Evelyn.Core.Tests.ReadModel.EventStream.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using Evelyn.Core.ReadModel.EventStream;
    using Evelyn.Core.ReadModel.EventStream.Handlers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Polly;
    using TestStack.BDDfy;
    using Xunit;

    public class EventStreamHandlerSpecs
    {
        private readonly Fixture _dataFixture;
        private readonly ServiceProvider _serviceProvider;
        private CancellationTokenSource _stoppingTokenSource;

        private EventStream _eventStream;
        private EventStreamHandler _eventStreamHandler;
        private StubbedEventHandler _eventHandler;

        private List<EventEnvelope> _eventsAddedToStream;
        private EventEnvelope _eventBeingHandleAtTimeOfSutdownd;
        private Task _eventStreamHandlerStoppingTask;

        public EventStreamHandlerSpecs()
        {
            _dataFixture = new Fixture();
            _eventHandler = new StubbedEventHandler();
            _stoppingTokenSource = new CancellationTokenSource();

            IServiceCollection services = new ServiceCollection();
            services.AddHostedService<EventStreamHandler>();
            services.AddSingleton<IEventStreamFactory, EventStreamFactory>();
            services.AddSingleton<IEventHandler<EventStreamHandler>>(_eventHandler);
            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();
            _eventStreamHandler = _serviceProvider.GetService<IHostedService>() as EventStreamHandler;
            _eventStream = _serviceProvider.GetService<IEventStreamFactory>().GetEventStream<EventStreamHandler>();

            _eventsAddedToStream = new List<EventEnvelope>();
        }

        [Fact]
        public void EventAddedToStreamBeforeHandlerIsStarted()
        {
            try
            {
                this.Given(_ => _.GivenMultipleEventsAreAddedToTheEventStream())
                    .When(_ => _.WhenTheEventStreamHandlerIsStarted())
                    .Then(_ => _.ThenAllTheEventsAreHandledByTheEventHandlerInTheCorrectOrder())
                    .And(_ => _.ThenThereAreNoQueuedEventsInTheEventStream())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void EventAddedToStreamAfterHandlerIsStarted()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .When(_ => _.WhenAnEventIsAddedToTheEventStream())
                    .Then(_ => _.ThenTheEventIsHandledByTheEventHandler())
                    .And(_ => _.ThenThereAreNoQueuedEventsInTheEventStream())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void ExceptionThrownByEventHandler()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .And(_ => _.GivenTheEventHandlerWillThrowAnException())
                    .When(_ => _.WhenMultipleEventsAreAddedToTheEventStream())
                    .Then(_ => _.ThenTheEventStreamHandlerStops())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void StoppingWhenNoEventsInStream()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .When(_ => _.WhenTheEventStreamHandlerIsStopped())
                    .Then(_ => _.ThenTheEventStreamHandlerStops())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        [Fact]
        public void StoppingWhenEventsInStream()
        {
            try
            {
                this.Given(_ => _.GivenTheEventStreamHandlerIsStarted())
                    .And(_ => _.GivenThereAreManyEventsInTheStream())
                    .And(_ => _.GivenTheCurrentEventWillTakeAWhileToBeHandled())
                    .When(_ => _.WhenTheEventStreamHandlerIsStopped())
                    .And(_ => _.WhenTheCurrentEventIsCompleted())
                    .Then(_ => _.ThenTheEventStreamHandlerStops())
                    .And(_ => _.TheEventHandlerDoesntProcessAnyMoreEventsFromTheQueue())
                    .BDDfy();
            }
            finally
            {
                _stoppingTokenSource.Cancel();
            }
        }

        private void GivenTheEventHandlerWillThrowAnException()
        {
            _eventHandler.ThrowOnHandleEventAttempt(1);
        }

        private async Task GivenTheEventStreamHandlerIsStarted()
        {
            await StartEventStreamHandler();
        }

        private void GivenMultipleEventsAreAddedToTheEventStream()
        {
            AddEventsToTheEventStream();
        }

        private void GivenThereAreManyEventsInTheStream()
        {
            AddEventsToTheEventStream(1000);
        }

        private void GivenTheCurrentEventWillTakeAWhileToBeHandled()
        {
            _eventHandler.Block();

            while (_eventBeingHandleAtTimeOfSutdownd == null)
            {
                _eventBeingHandleAtTimeOfSutdownd = _eventHandler.CurrentEvent;
            }
        }

        private async Task WhenTheEventStreamHandlerIsStarted()
        {
            await StartEventStreamHandler();
        }

        private void WhenAnEventIsAddedToTheEventStream()
        {
            var @event = new EventEnvelope(
                _dataFixture.Create<long>(),
                new SomeEvent());

            _eventStream.Enqueue(@event);

            _eventsAddedToStream.Add(@event);
        }

        private void WhenMultipleEventsAreAddedToTheEventStream()
        {
            AddEventsToTheEventStream();
        }

        private void WhenTheCurrentEventIsCompleted()
        {
            _eventHandler.Unblock();
        }

        private void WhenTheEventStreamHandlerIsStopped()
        {
            _eventStreamHandlerStoppingTask = _eventStreamHandler.StopAsync(default);
        }

        private void ThenTheEventIsHandledByTheEventHandler()
        {
            AssertHandledEvents(new[] { _eventsAddedToStream.First() });
        }

        private void ThenAllTheEventsAreHandledByTheEventHandlerInTheCorrectOrder()
        {
            AssertHandledEvents(_eventsAddedToStream);
        }

        private void TheEventHandlerDoesntProcessAnyMoreEventsFromTheQueue()
        {
            AssertLastHandledEvent(_eventBeingHandleAtTimeOfSutdownd);
            _eventHandler.HandledEvents.Count().Should().BeLessThan(_eventsAddedToStream.Count());
        }

        private async Task ThenTheEventStreamHandlerStops()
        {
            if (_eventStreamHandlerStoppingTask != null)
            {
                await _eventStreamHandlerStoppingTask;
            }

            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(50, retryAttempt => TimeSpan.FromMilliseconds(10));

            policy.Execute(() =>
            {
                _eventStreamHandler.Status.Should().Be(EventStreamHandlerStatus.Stopped);
            });
        }

        private async Task StartEventStreamHandler()
        {
            await _eventStreamHandler.StartAsync(_stoppingTokenSource.Token);
        }

        private void AddEventsToTheEventStream(int quantity = 2)
        {
            var events = new List<EventEnvelope>();

            int added = 0;
            while (added < quantity)
            {
                events.Add(
                    new EventEnvelope(
                    _dataFixture.Create<long>(),
                    new SomeEvent()));
                added++;
            }

            _eventStream.EnqueueRange(events);
            _eventsAddedToStream.AddRange(events);
        }

        private void AssertHandledEvents(IEnumerable<EventEnvelope> expectedOrderedEvents)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(50, retryAttempt => TimeSpan.FromMilliseconds(10));

            policy.Execute(() =>
            {
                _eventHandler.HandledEvents.Should().ContainInOrder(expectedOrderedEvents);
            });
        }

        private void AssertLastHandledEvent(EventEnvelope expectedLastHandledEvent)
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetry(50, retryAttempt => TimeSpan.FromMilliseconds(10));

            policy.Execute(() =>
            {
                _eventHandler.HandledEvents.Last().Should().Be(expectedLastHandledEvent);
            });
        }

        private void ThenThereAreNoQueuedEventsInTheEventStream()
        {
            _eventStream.QueueSize.Should().Be(0);
        }

        private class SomeEvent : CQRSlite.Events.IEvent
        {
            public Guid Id { get; set; }

            public int Version { get; set; }

            public DateTimeOffset TimeStamp { get; set; }
        }

        private class StubbedEventHandler : IEventHandler<EventStreamHandler>
        {
            private int _handleEventAttempt = 0;
            private List<int> _throwOnHandleEventAttempts = new List<int>();
            private List<EventEnvelope> _handledEvents = new List<EventEnvelope>();
            private bool _block = false;

            public IList<EventEnvelope> HandledEvents => _handledEvents;

            public EventEnvelope CurrentEvent { get; private set; }

            public void ThrowOnHandleEventAttempt(int handleEventAttemptNumber)
            {
                _throwOnHandleEventAttempts.Add(handleEventAttemptNumber);
            }

            public void Block()
            {
                _block = true;
            }

            public void Unblock()
            {
                _block = false;
            }

            public async Task HandleEvent(EventEnvelope eventEnvelope, CancellationToken stoppingToken)
            {
                _handleEventAttempt++;

                if (_throwOnHandleEventAttempts.Contains(_handleEventAttempt))
                {
                    throw new Exception("bang");
                }

                while (_block)
                {
                    CurrentEvent = eventEnvelope;
                    await Task.Delay(5);
                }

                await Task.Delay(5);

                CurrentEvent = null;

                _handledEvents.Add(eventEnvelope);
            }
        }
    }
}
